using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Data;

// I use this DbContext as the central map for tables, relationships, indexes, and seed data.

// The DbContext is EF Core's main entry point into the database.
// It defines which tables exist and how the entities relate to one another.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Each DbSet becomes a table-like entry point for queries and writes.
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<GameRelease> GameReleases => Set<GameRelease>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();
    public DbSet<ZeldaCatalogGame> ZeldaCatalogGames => Set<ZeldaCatalogGame>();
    public DbSet<ZeldaCatalogAvailability> ZeldaCatalogAvailabilities => Set<ZeldaCatalogAvailability>();
    public DbSet<ZeldaCatalogCollectionAppearance> ZeldaCatalogCollectionAppearances => Set<ZeldaCatalogCollectionAppearance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // These rules become database constraints and relationships.
        // They protect data integrity even if the frontend sends bad data.
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

        modelBuilder.Entity<CollectionItem>()
            .HasOne(item => item.AppUser)
            .WithMany(user => user.CollectionItems)
            .HasForeignKey(item => item.AppUserId);

        modelBuilder.Entity<CollectionItem>()
            .HasOne(item => item.Game)
            .WithMany(game => game.CollectionItems)
            .HasForeignKey(item => item.GameId);

        modelBuilder.Entity<CollectionItem>()
            .HasIndex(item => new { item.AppUserId, item.GameId })
            .IsUnique();
        
        modelBuilder.Entity<Comment>()
            .HasOne(comment => comment.AppUser)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.AppUserId);

        modelBuilder.Entity<Comment>()
            .HasOne(comment => comment.Game)
            .WithMany(game => game.Comments)
            .HasForeignKey(comment => comment.GameId);

        modelBuilder.Entity<Comment>()
            .HasIndex(comment => new { comment.GameId, comment.CreatedAt });

        modelBuilder.Entity<Reaction>()
            .HasOne(reaction => reaction.AppUser)
            .WithMany(user => user.Reactions)
            .HasForeignKey(reaction => reaction.AppUserId);

        modelBuilder.Entity<Reaction>()
            .HasOne(reaction => reaction.Game)
            .WithMany(game => game.Reactions)
            .HasForeignKey(reaction => reaction.GameId);

        // One user can only have one reaction per game.
        // If they change from Like to Dislike, update the existing row.
        modelBuilder.Entity<Reaction>()
            .HasIndex(reaction => new { reaction.AppUserId, reaction.GameId })
            .IsUnique();

        modelBuilder.Entity<ActivityEvent>()
            .HasOne(activity => activity.AppUser)
            .WithMany(user => user.ActivityEvents)
            .HasForeignKey(activity => activity.AppUserId);

        modelBuilder.Entity<ActivityEvent>()
            .HasOne(activity => activity.Game)
            .WithMany(game => game.ActivityEvents)
            .HasForeignKey(activity => activity.GameId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<ActivityEvent>()
            .HasIndex(activity => activity.CreatedAt);

        modelBuilder.Entity<ZeldaCatalogGame>()
            .HasIndex(game => game.Slug)
            .IsUnique();

        modelBuilder.Entity<ZeldaCatalogGame>()
            .HasIndex(game => game.MobyGameId)
            .IsUnique();

        modelBuilder.Entity<ZeldaCatalogAvailability>()
            .HasOne(availability => availability.ZeldaCatalogGame)
            .WithMany(game => game.AvailabilityEntries)
            .HasForeignKey(availability => availability.ZeldaCatalogGameId);

        modelBuilder.Entity<ZeldaCatalogAvailability>()
            .HasIndex(availability => new { availability.ZeldaCatalogGameId, availability.Kind, availability.DisplayOrder });

        modelBuilder.Entity<ZeldaCatalogCollectionAppearance>()
            .HasOne(appearance => appearance.ZeldaCatalogGame)
            .WithMany(game => game.CollectionAppearances)
            .HasForeignKey(appearance => appearance.ZeldaCatalogGameId);

        modelBuilder.Entity<ZeldaCatalogCollectionAppearance>()
            .HasIndex(appearance => new { appearance.ZeldaCatalogGameId, appearance.DisplayOrder });

        // SeedData adds a small starter catalog that exists before any simulated or real users interact.
        SeedData(modelBuilder);
        SeedZeldaCatalogData(modelBuilder);
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

    private static void SeedZeldaCatalogData(ModelBuilder modelBuilder)
    {
        static Guid CatalogGameId(int index) => Guid.Parse($"50000000-0000-0000-0000-{index:000000000000}");
        static Guid AvailabilityId(int index) => Guid.Parse($"60000000-0000-0000-0000-{index:000000000000}");
        static Guid AppearanceId(int index) => Guid.Parse($"70000000-0000-0000-0000-{index:000000000000}");

        var games = new[]
        {
            // I keep the original release date with each curated game so the catalog can be returned chronologically.
            new ZeldaCatalogGame { Id = CatalogGameId(1), Slug = "the-legend-of-zelda", MobyGameId = 3393, Title = "The Legend of Zelda", BoxArtPath = "/boxart/zeldanes.jpeg", OriginalReleaseDate = new DateTime(1986, 2, 21) },
            new ZeldaCatalogGame { Id = CatalogGameId(2), Slug = "zelda-ii-the-adventure-of-link", MobyGameId = 7296, Title = "Zelda II: The Adventure of Link", BoxArtPath = "/boxart/zelda2.jpg", OriginalReleaseDate = new DateTime(1987, 1, 14) },
            new ZeldaCatalogGame { Id = CatalogGameId(3), Slug = "a-link-to-the-past", MobyGameId = 6608, Title = "The Legend of Zelda: A Link to the Past", BoxArtPath = "/boxart/aLinktoThePast.jpg", OriginalReleaseDate = new DateTime(1991, 11, 21) },
            new ZeldaCatalogGame { Id = CatalogGameId(4), Slug = "links-awakening", MobyGameId = 4267, Title = "The Legend of Zelda: Link's Awakening", BoxArtPath = "/boxart/linksAwakening.png", OriginalReleaseDate = new DateTime(1993, 6, 6) },
            new ZeldaCatalogGame { Id = CatalogGameId(5), Slug = "ocarina-of-time", MobyGameId = 3549, Title = "The Legend of Zelda: Ocarina of Time", BoxArtPath = "/boxart/ocarinaOfTime.jpg", OriginalReleaseDate = new DateTime(1998, 11, 21) },
            new ZeldaCatalogGame { Id = CatalogGameId(6), Slug = "majoras-mask", MobyGameId = 3550, Title = "The Legend of Zelda: Majora's Mask", BoxArtPath = "/boxart/majorasMask.jpg", OriginalReleaseDate = new DateTime(2000, 4, 27) },
            new ZeldaCatalogGame { Id = CatalogGameId(17), Slug = "oracle-of-seasons", MobyGameId = 4269, Title = "The Legend of Zelda: Oracle of Seasons", BoxArtPath = "/boxart/oracleOfSeasons.webp", OriginalReleaseDate = new DateTime(2001, 2, 27) },
            new ZeldaCatalogGame { Id = CatalogGameId(18), Slug = "oracle-of-ages", MobyGameId = 4270, Title = "The Legend of Zelda: Oracle of Ages", BoxArtPath = "/boxart/oracleOfAges.webp", OriginalReleaseDate = new DateTime(2001, 2, 27) },
            new ZeldaCatalogGame { Id = CatalogGameId(7), Slug = "the-wind-waker", MobyGameId = 8725, Title = "The Legend of Zelda: The Wind Waker", BoxArtPath = "/boxart/windWaker.webp", OriginalReleaseDate = new DateTime(2002, 12, 13) },
            new ZeldaCatalogGame { Id = CatalogGameId(8), Slug = "twilight-princess", MobyGameId = 25103, Title = "The Legend of Zelda: Twilight Princess", BoxArtPath = "/boxart/twilightPrincess.jpg", OriginalReleaseDate = new DateTime(2006, 11, 19) },
            new ZeldaCatalogGame { Id = CatalogGameId(9), Slug = "phantom-hourglass", MobyGameId = 30522, Title = "The Legend of Zelda: Phantom Hourglass", BoxArtPath = "/boxart/phantomHourglass.jpg", OriginalReleaseDate = new DateTime(2007, 6, 23) },
            new ZeldaCatalogGame { Id = CatalogGameId(10), Slug = "spirit-tracks", MobyGameId = 44059, Title = "The Legend of Zelda: Spirit Tracks", BoxArtPath = "/boxart/spiritTracks.jpg", OriginalReleaseDate = new DateTime(2009, 12, 7) },
            new ZeldaCatalogGame { Id = CatalogGameId(11), Slug = "skyward-sword", MobyGameId = 53708, Title = "The Legend of Zelda: Skyward Sword", BoxArtPath = "/boxart/skywardSword.webp", OriginalReleaseDate = new DateTime(2011, 11, 18) },
            new ZeldaCatalogGame { Id = CatalogGameId(12), Slug = "a-link-between-worlds", MobyGameId = 62785, Title = "The Legend of Zelda: A Link Between Worlds", BoxArtPath = "/boxart/aLinkbetweenWorlds.jpg", OriginalReleaseDate = new DateTime(2013, 11, 22) },
            new ZeldaCatalogGame { Id = CatalogGameId(13), Slug = "tri-force-heroes", MobyGameId = 75907, Title = "The Legend of Zelda: Tri Force Heroes", BoxArtPath = "/boxart/triforceHeroes.avif", OriginalReleaseDate = new DateTime(2015, 10, 22) },
            new ZeldaCatalogGame { Id = CatalogGameId(14), Slug = "breath-of-the-wild", MobyGameId = 84595, Title = "The Legend of Zelda: Breath of the Wild", BoxArtPath = "/boxart/breathOfTheWild.jpg", OriginalReleaseDate = new DateTime(2017, 3, 3) },
            new ZeldaCatalogGame { Id = CatalogGameId(15), Slug = "tears-of-the-kingdom", MobyGameId = 203119, Title = "The Legend of Zelda: Tears of the Kingdom", BoxArtPath = "/boxart/tearsOfTheKingdom.jpg", OriginalReleaseDate = new DateTime(2023, 5, 12) },
            new ZeldaCatalogGame { Id = CatalogGameId(16), Slug = "echoes-of-wisdom", MobyGameId = 231129, Title = "The Legend of Zelda: Echoes of Wisdom", BoxArtPath = "/boxart/echoesOfWisdom.avif", OriginalReleaseDate = new DateTime(2024, 9, 26) },
        };

        modelBuilder.Entity<ZeldaCatalogGame>().HasData(games);

        var availabilities = new[]
        {
            new ZeldaCatalogAvailability { Id = AvailabilityId(1), ZeldaCatalogGameId = CatalogGameId(1), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "NES", ImagePath = "/platformLogos/nesLogo.png", Note = "Original 1986 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(2), ZeldaCatalogGameId = CatalogGameId(1), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Game Boy Advance", ImagePath = "/platformLogos/gbaLogo.png", Note = "Classic NES Series port" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(3), ZeldaCatalogGameId = CatalogGameId(1), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii Virtual Console", ImagePath = "/platformLogos/WiiLogo.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(4), ZeldaCatalogGameId = CatalogGameId(1), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 2, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(5), ZeldaCatalogGameId = CatalogGameId(1), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 3, Name = "Nintendo 3DS Virtual Console", ImagePath = "/platformLogos/3dsVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(6), ZeldaCatalogGameId = CatalogGameId(1), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(7), ZeldaCatalogGameId = CatalogGameId(2), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "NES", ImagePath = "/platformLogos/nesLogo.png", Note = "Original 1987 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(8), ZeldaCatalogGameId = CatalogGameId(2), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Game Boy Advance", ImagePath = "/platformLogos/gbaLogo.png", Note = "Classic NES Series port" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(9), ZeldaCatalogGameId = CatalogGameId(2), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii Virtual Console", ImagePath = "/platformLogos/WiiLogo.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(10), ZeldaCatalogGameId = CatalogGameId(2), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 2, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(11), ZeldaCatalogGameId = CatalogGameId(2), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 3, Name = "Nintendo 3DS Virtual Console", ImagePath = "/platformLogos/3dsVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(12), ZeldaCatalogGameId = CatalogGameId(2), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(13), ZeldaCatalogGameId = CatalogGameId(3), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "SNES", ImagePath = "/platformLogos/snesLogo.png", Note = "Original 1991 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(14), ZeldaCatalogGameId = CatalogGameId(3), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Game Boy Advance", ImagePath = "/platformLogos/gbaLogo.png", Note = "Game Boy Advance port with Four Swords bundle" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(15), ZeldaCatalogGameId = CatalogGameId(3), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii Virtual Console", ImagePath = "/platformLogos/WiiLogo.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(16), ZeldaCatalogGameId = CatalogGameId(3), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 2, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(17), ZeldaCatalogGameId = CatalogGameId(3), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 3, Name = "Nintendo 3DS Virtual Console", ImagePath = "/platformLogos/3dsVirtualConsole.png", Note = "New Nintendo 3DS Virtual Console release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(18), ZeldaCatalogGameId = CatalogGameId(3), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png", Note = "Available in the SNES app" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(19), ZeldaCatalogGameId = CatalogGameId(4), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Game Boy", ImagePath = "/platformLogos/gbLogo.png", Note = "Original 1993 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(20), ZeldaCatalogGameId = CatalogGameId(4), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Game Boy Color", ImagePath = "/platformLogos/gbcLogo.png", Note = "Link's Awakening DX enhanced version" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(21), ZeldaCatalogGameId = CatalogGameId(4), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 2, Name = "Nintendo Switch", ImagePath = "/platformLogos/switchLogo.svg", Note = "Full Nintendo Switch remake" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(22), ZeldaCatalogGameId = CatalogGameId(4), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Nintendo 3DS Virtual Console", ImagePath = "/platformLogos/3dsVirtualConsole.png", Note = "3DS Virtual Console release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(23), ZeldaCatalogGameId = CatalogGameId(4), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png", Note = "Available in the Game Boy app" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(53), ZeldaCatalogGameId = CatalogGameId(17), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Game Boy Color", ImagePath = "/platformLogos/gbcLogo.png", Note = "Original 2001 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(54), ZeldaCatalogGameId = CatalogGameId(17), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Nintendo 3DS Virtual Console", ImagePath = "/platformLogos/3dsVirtualConsole.png", Note = "3DS Virtual Console release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(55), ZeldaCatalogGameId = CatalogGameId(17), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png", Note = "Available in the Game Boy app" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(56), ZeldaCatalogGameId = CatalogGameId(18), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Game Boy Color", ImagePath = "/platformLogos/gbcLogo.png", Note = "Original 2001 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(57), ZeldaCatalogGameId = CatalogGameId(18), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Nintendo 3DS Virtual Console", ImagePath = "/platformLogos/3dsVirtualConsole.png", Note = "3DS Virtual Console release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(58), ZeldaCatalogGameId = CatalogGameId(18), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png", Note = "Available in the Game Boy app" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(24), ZeldaCatalogGameId = CatalogGameId(5), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo 64", ImagePath = "/platformLogos/n64Logo.png", Note = "Original 1998 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(25), ZeldaCatalogGameId = CatalogGameId(5), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Nintendo 3DS", ImagePath = "/platformLogos/3dsLogo.svg", Note = "Ocarina of Time 3D remake" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(26), ZeldaCatalogGameId = CatalogGameId(5), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii Virtual Console", ImagePath = "/platformLogos/WiiLogo.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(27), ZeldaCatalogGameId = CatalogGameId(5), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 2, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(28), ZeldaCatalogGameId = CatalogGameId(5), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png", Note = "Available in the Nintendo 64 app" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(29), ZeldaCatalogGameId = CatalogGameId(6), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo 64", ImagePath = "/platformLogos/n64Logo.png", Note = "Original 2000 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(30), ZeldaCatalogGameId = CatalogGameId(6), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Nintendo 3DS", ImagePath = "/platformLogos/3dsLogo.svg", Note = "Majora's Mask 3D remake" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(31), ZeldaCatalogGameId = CatalogGameId(6), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii Virtual Console", ImagePath = "/platformLogos/WiiLogo.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(32), ZeldaCatalogGameId = CatalogGameId(6), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 2, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(33), ZeldaCatalogGameId = CatalogGameId(6), Kind = ZeldaCatalogAvailabilityKind.SubscriptionService, DisplayOrder = 1, Name = "Nintendo Switch Online", ImagePath = "/platformLogos/nintendoSwitchOnlineLogo.png", Note = "Available in the Nintendo 64 app" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(34), ZeldaCatalogGameId = CatalogGameId(7), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "GameCube", ImagePath = "/platformLogos/gamecubeLogo.jpeg", Note = "Original 2002 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(35), ZeldaCatalogGameId = CatalogGameId(7), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Wii U", ImagePath = "/platformLogos/wiiuLogo.png", Note = "The Wind Waker HD remaster" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(36), ZeldaCatalogGameId = CatalogGameId(8), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "GameCube", ImagePath = "/platformLogos/gamecubeLogo.jpeg", Note = "Original release family" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(37), ZeldaCatalogGameId = CatalogGameId(8), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Wii", ImagePath = "/platformLogos/WiiLogo.png", Note = "Simultaneous launch-era version" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(38), ZeldaCatalogGameId = CatalogGameId(8), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 2, Name = "Wii U", ImagePath = "/platformLogos/wiiuLogo.png", Note = "Twilight Princess HD remaster" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(39), ZeldaCatalogGameId = CatalogGameId(9), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo DS", ImagePath = "/platformLogos/ndsLogo.png", Note = "Original 2007 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(40), ZeldaCatalogGameId = CatalogGameId(9), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png", Note = "Wii U download release" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(41), ZeldaCatalogGameId = CatalogGameId(10), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo DS", ImagePath = "/platformLogos/ndsLogo.png", Note = "Original 2009 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(42), ZeldaCatalogGameId = CatalogGameId(10), Kind = ZeldaCatalogAvailabilityKind.DownloadedPort, DisplayOrder = 1, Name = "Wii U Virtual Console", ImagePath = "/platformLogos/wiiUVirtualConsole.png", Note = "Wii U download release" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(43), ZeldaCatalogGameId = CatalogGameId(11), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Wii", ImagePath = "/platformLogos/WiiLogo.png", Note = "Original 2011 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(44), ZeldaCatalogGameId = CatalogGameId(11), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Nintendo Switch", ImagePath = "/platformLogos/switchLogo.svg", Note = "Skyward Sword HD remaster" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(45), ZeldaCatalogGameId = CatalogGameId(12), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo 3DS", ImagePath = "/platformLogos/3dsLogo.svg", Note = "Original 2013 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(46), ZeldaCatalogGameId = CatalogGameId(13), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo 3DS", ImagePath = "/platformLogos/3dsLogo.svg", Note = "Original 2015 release" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(47), ZeldaCatalogGameId = CatalogGameId(14), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Wii U", ImagePath = "/platformLogos/wiiuLogo.png", Note = "Launch release family lead platform" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(48), ZeldaCatalogGameId = CatalogGameId(14), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Nintendo Switch", ImagePath = "/platformLogos/switchLogo.svg", Note = "Launch-day Nintendo Switch release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(49), ZeldaCatalogGameId = CatalogGameId(14), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 2, Name = "Nintendo Switch 2", ImagePath = "/platformLogos/switch2Logo.png", Note = "Nintendo Switch 2 version" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(50), ZeldaCatalogGameId = CatalogGameId(15), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo Switch", ImagePath = "/platformLogos/switchLogo.svg", Note = "Original 2023 release" },
            new ZeldaCatalogAvailability { Id = AvailabilityId(51), ZeldaCatalogGameId = CatalogGameId(15), Kind = ZeldaCatalogAvailabilityKind.SecondaryConsole, DisplayOrder = 1, Name = "Nintendo Switch 2", ImagePath = "/platformLogos/switch2Logo.png", Note = "Nintendo Switch 2 version" },

            new ZeldaCatalogAvailability { Id = AvailabilityId(52), ZeldaCatalogGameId = CatalogGameId(16), Kind = ZeldaCatalogAvailabilityKind.PrimaryConsole, DisplayOrder = 1, Name = "Nintendo Switch", ImagePath = "/platformLogos/switchLogo.svg", Note = "Original 2024 release" },
        };

        modelBuilder.Entity<ZeldaCatalogAvailability>().HasData(availabilities);

        var appearances = new[]
        {
            new ZeldaCatalogCollectionAppearance { Id = AppearanceId(1), ZeldaCatalogGameId = CatalogGameId(1), DisplayOrder = 1, Title = "The Legend of Zelda: Collector's Edition", BoxArtPath = "/boxart/zeldaCollectorEdition.webp", PlatformName = "GameCube", PlatformImagePath = "/platformLogos/gamecubeLogo.jpeg", Note = "Full game included on the compilation disc" },
            new ZeldaCatalogCollectionAppearance { Id = AppearanceId(2), ZeldaCatalogGameId = CatalogGameId(2), DisplayOrder = 1, Title = "The Legend of Zelda: Collector's Edition", BoxArtPath = "/boxart/zeldaCollectorEdition.webp", PlatformName = "GameCube", PlatformImagePath = "/platformLogos/gamecubeLogo.jpeg", Note = "Full game included on the compilation disc" },
            new ZeldaCatalogCollectionAppearance { Id = AppearanceId(3), ZeldaCatalogGameId = CatalogGameId(5), DisplayOrder = 1, Title = "The Legend of Zelda: Collector's Edition", BoxArtPath = "/boxart/zeldaCollectorEdition.webp", PlatformName = "GameCube", PlatformImagePath = "/platformLogos/gamecubeLogo.jpeg", Note = "Full game included on the compilation disc" },
            new ZeldaCatalogCollectionAppearance { Id = AppearanceId(4), ZeldaCatalogGameId = CatalogGameId(6), DisplayOrder = 1, Title = "The Legend of Zelda: Collector's Edition", BoxArtPath = "/boxart/zeldaCollectorEdition.webp", PlatformName = "GameCube", PlatformImagePath = "/platformLogos/gamecubeLogo.jpeg", Note = "Full game included on the compilation disc" },
        };

        modelBuilder.Entity<ZeldaCatalogCollectionAppearance>().HasData(appearances);
    }
}