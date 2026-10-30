using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HyruleArchives.Data;

public class PostgresAppDbContext : AppDbContext
{
    public PostgresAppDbContext(DbContextOptions<PostgresAppDbContext> options): base(options) {
    }
}

// Lets the migration tool work without starting the website.
public class PostgresAppDbContextFactory
    : IDesignTimeDbContextFactory<PostgresAppDbContext>
{
    public PostgresAppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PostgresAppDbContext>();

        // Placeholder only: generating migrations doesn't connect to Neon.
        options.UseNpgsql(
            "Host=localhost;Database=hyrule_migrations;Username=postgres");

        return new PostgresAppDbContext(options.Options);
    }
}