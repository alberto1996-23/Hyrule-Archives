using HyruleArchives.Data;
using HyruleArchives.Endpoints;
using HyruleArchives.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using HyruleArchives.Hubs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=hyrule-archives.db";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<AuthUserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5145",
                "https://localhost:5145"
            )
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
app.MapPlatformEndpoints();
app.MapRegionEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapCollectionEndpoints();
app.MapMobyGamesEndpoints();
app.MapActivityEndpoints();
app.MapMyActivityEndpoints();
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

