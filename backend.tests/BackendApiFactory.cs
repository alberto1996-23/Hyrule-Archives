using HyruleArchives.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HyruleArchives.Tests;

// Each factory instance gets its own SQLite database file so tests stay isolated.
public sealed class BackendApiFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"hyrule-archives-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "Sqlite",
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_databasePath}",
                ["Frontend:BaseUrl"] = "http://localhost:3000"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={_databasePath}");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthHandler.SchemeName;
                options.DefaultScheme = TestAuthHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    public HttpClient CreateAuthenticatedClient(string subjectId, string? displayName = null, string? email = null)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectIdHeader, subjectId);
        client.DefaultRequestHeaders.Add(TestAuthHandler.DisplayNameHeader, displayName ?? $"Test User {subjectId}");
        client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, email ?? $"{subjectId}@tests.local");

        return client;
    }

    public async Task SeedAsync(Func<AppDbContext, Task> seedAction)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await seedAction(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
