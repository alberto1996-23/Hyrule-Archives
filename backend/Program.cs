using HyruleArchives.Data;
using HyruleArchives.Endpoints;
using HyruleArchives.Infrastructure;
using HyruleArchives.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using HyruleArchives.Hubs;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Resolve the database provider up front so the same startup code can run
// against local SQLite or production PostgreSQL without branching elsewhere.
var connectionString = ResolveConnectionString(builder.Configuration);
var databaseProvider = ResolveDatabaseProvider(builder.Configuration, connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider == DatabaseProvider.PostgreSql)
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

builder.Services.AddScoped<AuthUserService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<ZeldaCatalogService>();

builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.Cookie.Name = "HyruleArchives.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;
    options.Events = new CookieAuthenticationEvents
    {
        // Frontend fetch calls should receive plain HTTP status codes instead of HTML/login redirects.
        // Without this, a cross-origin redirect to the login flow can surface as "Failed to fetch"
        // after logout or session expiry.
        OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api") ||
                context.Request.Headers.Accept.Any(value => value.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api") ||
                context.Request.Headers.Accept.Any(value => value.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

if (!string.IsNullOrWhiteSpace(googleClientId) &&
    !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder.AddGoogle("Google", options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.CallbackPath = "/auth/callback/google";
        options.SaveTokens = false;
    });
}

builder.Services.AddAuthorization();

var allowedOrigins = (builder.Configuration
        .GetSection("Frontend:AllowedOrigins")
        .Get<string[]>() ?? [])
    .Append(builder.Configuration["Frontend:BaseUrl"])
    .Concat(
    [
        "http://localhost:3000",
        "https://localhost:3000",
        "http://localhost:5145",
        "https://localhost:5145"
    ])
    .OfType<string>()
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Registers the MobyGames service with HttpClient.
// This lets the service make HTTP requests to the MobyGames API.
builder.Services.AddHttpClient<MobyGamesService>();

// Registers the seed service.
// This is used only for local development seed data.
builder.Services.AddScoped<SeedService>();

// Adds SignalR services to the backend.
// SignalR lets the server push live updates to connected React clients.
builder.Services.AddSignalR();

// Handles creating activity records and broadcasting them through SignalR.
builder.Services.AddScoped<ActivityEventService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        app = "The Hyrule Archives",
        timestamp = DateTimeOffset.UtcNow
    });
})
.WithTags("Health")
.WithName("HealthCheck")
.WithSummary("Checks whether the backend is running.");

app.MapGameEndpoints();
app.MapCatalogEndpoints();
app.MapPlatformEndpoints();
app.MapRegionEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapCollectionEndpoints();
app.MapMobyGamesEndpoints();
app.MapActivityEndpoints();
app.MapMyActivityEndpoints();
app.MapCommunityEndpoints();
app.MapCommentEndpoints();
app.MapReactionEndpoints();

// Exposes the SignalR activity feed hub.
// React will connect to this URL later:
// http://localhost:5154/hubs/activity
app.MapHub<ActivityHub>("/hubs/activity");

// Only expose seed endpoints during local development.
if (app.Environment.IsDevelopment())
{
    app.MapDevSeedEndpoints();
}

app.Run();

static string ResolveConnectionString(IConfiguration configuration)
{
    var configuredConnectionString = configuration.GetConnectionString("DefaultConnection");

    if (!string.IsNullOrWhiteSpace(configuredConnectionString))
    {
        return configuredConnectionString;
    }

    var databaseUrl = configuration["DATABASE_URL"];

    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        return ConvertDatabaseUrlToConnectionString(databaseUrl);
    }

    return "Data Source=hyrule-archives.db";
}

static string ResolveDatabaseProvider(IConfiguration configuration, string connectionString)
{
    var configuredProvider = configuration["Database:Provider"];

    if (string.Equals(configuredProvider, DatabaseProvider.PostgreSql, StringComparison.OrdinalIgnoreCase))
    {
        return DatabaseProvider.PostgreSql;
    }

    if (string.Equals(configuredProvider, DatabaseProvider.Sqlite, StringComparison.OrdinalIgnoreCase))
    {
        return DatabaseProvider.Sqlite;
    }

    return connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
        ? DatabaseProvider.PostgreSql
        : DatabaseProvider.Sqlite;
}

static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri) ||
        (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
    {
        return databaseUrl;
    }

    var userInfo = uri.UserInfo.Split(':', 2, StringSplitOptions.TrimEntries);

    var connectionStringBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.Trim('/'),
        Username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty,
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
        SslMode = SslMode.Require
    };

    return connectionStringBuilder.ConnectionString;
}

