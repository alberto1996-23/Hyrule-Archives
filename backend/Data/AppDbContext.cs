using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<GameRelease> GameReleases => Set<GameRelease>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasIndex(user => new { user.OAuthProvider, user.OAuthSubjectId })
            .IsUnique();

        modelBuilder.Entity<Game>()
            .HasIndex(game => game.Slug)
            .IsUnique();

        modelBuilder.Entity<Platform>()
            .HasIndex(platform => platform.Slug)
            .IsUnique();

        modelBuilder.Entity<Region>()
            .HasIndex(region => region.Code)
            .IsUnique();

        modelBuilder.Entity<GameRelease>()
            .HasOne(release => release.Game)
            .WithMany(game => game.Releases)
            .HasForeignKey(release => release.GameId);

        modelBuilder.Entity<GameRelease>()
            .HasOne(release => release.Platform)
            .WithMany(platform => platform.Releases)
            .HasForeignKey(release => release.PlatformId);

        modelBuilder.Entity<GameRelease>()
            .HasOne(release => release.Region)
            .WithMany(region => region.Releases)
            .HasForeignKey(release => release.RegionId);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedCreatedAt = new DateTimeOffset(2026, 5, 22, 0, 0, 0, TimeSpan.Zero);

        var nesId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var snesId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var n64Id = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var switchId = Guid.Parse("10000000-0000-0000-0000-000000000004");

        var japanId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var northAmericaId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var europeId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var globalId = Guid.Parse("20000000-0000-0000-0000-000000000004");

        var zelda1Id = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var linkToPastId = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var ocarinaId = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var botwId = Guid.Parse("30000000-0000-0000-0000-000000000004");
        var totkId = Guid.Parse("30000000-0000-0000-0000-000000000005");

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = nesId, Name = "Nintendo Entertainment System", Slug = "nes" },
            new Platform { Id = snesId, Name = "Super Nintendo Entertainment System", Slug = "snes" },
            new Platform { Id = n64Id, Name = "Nintendo 64", Slug = "nintendo-64" },
            new Platform { Id = switchId, Name = "Nintendo Switch", Slug = "nintendo-switch" }
        );

        modelBuilder.Entity<Region>().HasData(
            new Region { Id = japanId, Name = "Japan", Code = "JP" },
            new Region { Id = northAmericaId, Name = "North America", Code = "NA" },
            new Region { Id = europeId, Name = "Europe", Code = "EU" },
            new Region { Id = globalId, Name = "Global", Code = "GLOBAL" }
        );

        modelBuilder.Entity<Game>().HasData(
            new Game
            {
                Id = zelda1Id,
                ExternalApiId = "rawg-placeholder-zelda-1",
                Title = "The Legend of Zelda",
                Slug = "the-legend-of-zelda",
                Summary = "The first game in the series, introducing players to Link, Princess Zelda, Hyrule, exploration, dungeons, and the Triforce.",
                ImageUrl = null,
                CreatedAt = seedCreatedAt
            },
            new Game
            {
                Id = linkToPastId,
                ExternalApiId = "rawg-placeholder-link-to-the-past",
                Title = "The Legend of Zelda: A Link to the Past",
                Slug = "a-link-to-the-past",
                Summary = "A major Super Nintendo entry that expanded Hyrule with the Light World and Dark World structure.",
                ImageUrl = null,
                CreatedAt = seedCreatedAt
            },
            new Game
            {
                Id = ocarinaId,
                ExternalApiId = "rawg-placeholder-ocarina-of-time",
                Title = "The Legend of Zelda: Ocarina of Time",
                Slug = "ocarina-of-time",
                Summary = "A landmark 3D Zelda game that follows Link across time as he tries to stop Ganondorf and protect Hyrule.",
                ImageUrl = null,
                CreatedAt = seedCreatedAt
            },
            new Game
            {
                Id = botwId,
                ExternalApiId = "rawg-placeholder-breath-of-the-wild",
                Title = "The Legend of Zelda: Breath of the Wild",
                Slug = "breath-of-the-wild",
                Summary = "An open-world Zelda game focused on exploration, survival, physics-based systems, and the ruined kingdom of Hyrule.",
                ImageUrl = null,
                CreatedAt = seedCreatedAt
            },
            new Game
            {
                Id = totkId,
                ExternalApiId = "rawg-placeholder-tears-of-the-kingdom",
                Title = "The Legend of Zelda: Tears of the Kingdom",
                Slug = "tears-of-the-kingdom",
                Summary = "A sequel to Breath of the Wild that expands Hyrule with sky islands, underground areas, and new building mechanics.",
                ImageUrl = null,
                CreatedAt = seedCreatedAt
            }
        );

        modelBuilder.Entity<GameRelease>().HasData(
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                GameId = zelda1Id,
                PlatformId = nesId,
                RegionId = japanId,
                ReleaseDate = new DateTime(1986, 2, 21),
                Source = "Seed Data"
            },
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                GameId = zelda1Id,
                PlatformId = nesId,
                RegionId = northAmericaId,
                ReleaseDate = new DateTime(1987, 8, 22),
                Source = "Seed Data"
            },
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                GameId = linkToPastId,
                PlatformId = snesId,
                RegionId = japanId,
                ReleaseDate = new DateTime(1991, 11, 21),
                Source = "Seed Data"
            },
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                GameId = ocarinaId,
                PlatformId = n64Id,
                RegionId = japanId,
                ReleaseDate = new DateTime(1998, 11, 21),
                Source = "Seed Data"
            },
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000005"),
                GameId = ocarinaId,
                PlatformId = n64Id,
                RegionId = northAmericaId,
                ReleaseDate = new DateTime(1998, 11, 23),
                Source = "Seed Data"
            },
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000006"),
                GameId = botwId,
                PlatformId = switchId,
                RegionId = globalId,
                ReleaseDate = new DateTime(2017, 3, 3),
                Source = "Seed Data"
            },
            new GameRelease
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000007"),
                GameId = totkId,
                PlatformId = switchId,
                RegionId = globalId,
                ReleaseDate = new DateTime(2023, 5, 12),
                Source = "Seed Data"
            }
        );
    }
}