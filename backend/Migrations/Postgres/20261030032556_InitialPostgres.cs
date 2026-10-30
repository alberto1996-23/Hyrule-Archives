using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OAuthProvider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OAuthSubjectId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsSimulated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalApiId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZeldaCatalogGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MobyGameId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BoxArtPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OriginalReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeldaCatalogGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEvents_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityEvents_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    FavoritedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionItems_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionItems_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reactions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameReleases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameReleases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameReleases_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameReleases_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameReleases_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZeldaCatalogAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZeldaCatalogGameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeldaCatalogAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZeldaCatalogAvailabilities_ZeldaCatalogGames_ZeldaCatalogGa~",
                        column: x => x.ZeldaCatalogGameId,
                        principalTable: "ZeldaCatalogGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZeldaCatalogCollectionAppearances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZeldaCatalogGameId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BoxArtPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PlatformName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PlatformImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeldaCatalogCollectionAppearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZeldaCatalogCollectionAppearances_ZeldaCatalogGames_ZeldaCa~",
                        column: x => x.ZeldaCatalogGameId,
                        principalTable: "ZeldaCatalogGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "CreatedAt", "ExternalApiId", "ImageUrl", "Slug", "Summary", "Title" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "rawg-placeholder-zelda-1", null, "the-legend-of-zelda", "The first game in the series, introducing players to Link, Princess Zelda, Hyrule, exploration, dungeons, and the Triforce.", "The Legend of Zelda" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "rawg-placeholder-link-to-the-past", null, "a-link-to-the-past", "A major Super Nintendo entry that expanded Hyrule with the Light World and Dark World structure.", "The Legend of Zelda: A Link to the Past" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "rawg-placeholder-ocarina-of-time", null, "ocarina-of-time", "A landmark 3D Zelda game that follows Link across time as he tries to stop Ganondorf and protect Hyrule.", "The Legend of Zelda: Ocarina of Time" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "rawg-placeholder-breath-of-the-wild", null, "breath-of-the-wild", "An open-world Zelda game focused on exploration, survival, physics-based systems, and the ruined kingdom of Hyrule.", "The Legend of Zelda: Breath of the Wild" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "rawg-placeholder-tears-of-the-kingdom", null, "tears-of-the-kingdom", "A sequel to Breath of the Wild that expands Hyrule with sky islands, underground areas, and new building mechanics.", "The Legend of Zelda: Tears of the Kingdom" }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Nintendo Entertainment System", "nes" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Super Nintendo Entertainment System", "snes" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Nintendo 64", "nintendo-64" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Nintendo Switch", "nintendo-switch" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "JP", "Japan" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "NA", "North America" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "EU", "Europe" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "GLOBAL", "Global" }
                });

            migrationBuilder.InsertData(
                table: "ZeldaCatalogGames",
                columns: new[] { "Id", "BoxArtPath", "MobyGameId", "OriginalReleaseDate", "Slug", "Title" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "/boxart/zeldanes.jpeg", 3393, new DateTime(1986, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "the-legend-of-zelda", "The Legend of Zelda" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "/boxart/zelda2.jpg", 7296, new DateTime(1987, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "zelda-ii-the-adventure-of-link", "Zelda II: The Adventure of Link" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "/boxart/aLinktoThePast.jpg", 6608, new DateTime(1991, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "a-link-to-the-past", "The Legend of Zelda: A Link to the Past" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "/boxart/linksAwakening.png", 4267, new DateTime(1993, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "links-awakening", "The Legend of Zelda: Link's Awakening" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "/boxart/ocarinaOfTime.jpg", 3549, new DateTime(1998, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "ocarina-of-time", "The Legend of Zelda: Ocarina of Time" },
                    { new Guid("50000000-0000-0000-0000-000000000006"), "/boxart/majorasMask.jpg", 3550, new DateTime(2000, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "majoras-mask", "The Legend of Zelda: Majora's Mask" },
                    { new Guid("50000000-0000-0000-0000-000000000007"), "/boxart/windWaker.webp", 8725, new DateTime(2002, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "the-wind-waker", "The Legend of Zelda: The Wind Waker" },
                    { new Guid("50000000-0000-0000-0000-000000000008"), "/boxart/twilightPrincess.jpg", 25103, new DateTime(2006, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "twilight-princess", "The Legend of Zelda: Twilight Princess" },
                    { new Guid("50000000-0000-0000-0000-000000000009"), "/boxart/phantomHourglass.jpg", 30522, new DateTime(2007, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "phantom-hourglass", "The Legend of Zelda: Phantom Hourglass" },
                    { new Guid("50000000-0000-0000-0000-000000000010"), "/boxart/spiritTracks.jpg", 44059, new DateTime(2009, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "spirit-tracks", "The Legend of Zelda: Spirit Tracks" },
                    { new Guid("50000000-0000-0000-0000-000000000011"), "/boxart/skywardSword.webp", 53708, new DateTime(2011, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "skyward-sword", "The Legend of Zelda: Skyward Sword" },
                    { new Guid("50000000-0000-0000-0000-000000000012"), "/boxart/aLinkbetweenWorlds.jpg", 62785, new DateTime(2013, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "a-link-between-worlds", "The Legend of Zelda: A Link Between Worlds" },
                    { new Guid("50000000-0000-0000-0000-000000000013"), "/boxart/triforceHeroes.avif", 75907, new DateTime(2015, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "tri-force-heroes", "The Legend of Zelda: Tri Force Heroes" },
                    { new Guid("50000000-0000-0000-0000-000000000014"), "/boxart/breathOfTheWild.jpg", 84595, new DateTime(2017, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "breath-of-the-wild", "The Legend of Zelda: Breath of the Wild" },
                    { new Guid("50000000-0000-0000-0000-000000000015"), "/boxart/tearsOfTheKingdom.jpg", 203119, new DateTime(2023, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "tears-of-the-kingdom", "The Legend of Zelda: Tears of the Kingdom" },
                    { new Guid("50000000-0000-0000-0000-000000000016"), "/boxart/echoesOfWisdom.avif", 231129, new DateTime(2024, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "echoes-of-wisdom", "The Legend of Zelda: Echoes of Wisdom" },
                    { new Guid("50000000-0000-0000-0000-000000000017"), "/boxart/oracleOfSeasons.webp", 4269, new DateTime(2001, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "oracle-of-seasons", "The Legend of Zelda: Oracle of Seasons" },
                    { new Guid("50000000-0000-0000-0000-000000000018"), "/boxart/oracleOfAges.webp", 4270, new DateTime(2001, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "oracle-of-ages", "The Legend of Zelda: Oracle of Ages" }
                });

            migrationBuilder.InsertData(
                table: "GameReleases",
                columns: new[] { "Id", "GameId", "PlatformId", "RegionId", "ReleaseDate", "Source" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(1986, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(1987, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(1991, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(1998, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(1998, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" },
                    { new Guid("40000000-0000-0000-0000-000000000006"), new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2017, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" },
                    { new Guid("40000000-0000-0000-0000-000000000007"), new Guid("30000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2023, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed Data" }
                });

            migrationBuilder.InsertData(
                table: "ZeldaCatalogAvailabilities",
                columns: new[] { "Id", "DisplayOrder", "ImagePath", "Kind", "Name", "Note", "ZeldaCatalogGameId" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), 1, "/platformLogos/nesLogo.png", 1, "NES", "Original 1986 release", new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000002"), 1, "/platformLogos/gbaLogo.png", 2, "Game Boy Advance", "Classic NES Series port", new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000003"), 1, "/platformLogos/WiiLogo.png", 3, "Wii Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000004"), 2, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000005"), 3, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000006"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", null, new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000007"), 1, "/platformLogos/nesLogo.png", 1, "NES", "Original 1987 release", new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000008"), 1, "/platformLogos/gbaLogo.png", 2, "Game Boy Advance", "Classic NES Series port", new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000009"), 1, "/platformLogos/WiiLogo.png", 3, "Wii Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000010"), 2, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000011"), 3, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000012"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", null, new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000013"), 1, "/platformLogos/snesLogo.png", 1, "SNES", "Original 1991 release", new Guid("50000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000014"), 1, "/platformLogos/gbaLogo.png", 2, "Game Boy Advance", "Game Boy Advance port with Four Swords bundle", new Guid("50000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000015"), 1, "/platformLogos/WiiLogo.png", 3, "Wii Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000016"), 2, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000017"), 3, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", "New Nintendo 3DS Virtual Console release", new Guid("50000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000018"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the SNES app", new Guid("50000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000019"), 1, "/platformLogos/gbLogo.png", 1, "Game Boy", "Original 1993 release", new Guid("50000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000020"), 1, "/platformLogos/gbcLogo.png", 2, "Game Boy Color", "Link's Awakening DX enhanced version", new Guid("50000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000021"), 2, "/platformLogos/switchLogo.svg", 2, "Nintendo Switch", "Full Nintendo Switch remake", new Guid("50000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000022"), 1, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", "3DS Virtual Console release", new Guid("50000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000023"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Game Boy app", new Guid("50000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000024"), 1, "/platformLogos/n64Logo.png", 1, "Nintendo 64", "Original 1998 release", new Guid("50000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000025"), 1, "/platformLogos/3dsLogo.svg", 2, "Nintendo 3DS", "Ocarina of Time 3D remake", new Guid("50000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000026"), 1, "/platformLogos/WiiLogo.png", 3, "Wii Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000027"), 2, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000028"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Nintendo 64 app", new Guid("50000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000029"), 1, "/platformLogos/n64Logo.png", 1, "Nintendo 64", "Original 2000 release", new Guid("50000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000030"), 1, "/platformLogos/3dsLogo.svg", 2, "Nintendo 3DS", "Majora's Mask 3D remake", new Guid("50000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000031"), 1, "/platformLogos/WiiLogo.png", 3, "Wii Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000032"), 2, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", null, new Guid("50000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000033"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Nintendo 64 app", new Guid("50000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000034"), 1, "/platformLogos/gamecubeLogo.jpeg", 1, "GameCube", "Original 2002 release", new Guid("50000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000035"), 1, "/platformLogos/wiiuLogo.png", 2, "Wii U", "The Wind Waker HD remaster", new Guid("50000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000036"), 1, "/platformLogos/gamecubeLogo.jpeg", 1, "GameCube", "Original release family", new Guid("50000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000037"), 1, "/platformLogos/WiiLogo.png", 2, "Wii", "Simultaneous launch-era version", new Guid("50000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000038"), 2, "/platformLogos/wiiuLogo.png", 2, "Wii U", "Twilight Princess HD remaster", new Guid("50000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000039"), 1, "/platformLogos/ndsLogo.png", 1, "Nintendo DS", "Original 2007 release", new Guid("50000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000040"), 1, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", "Wii U download release", new Guid("50000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000041"), 1, "/platformLogos/ndsLogo.png", 1, "Nintendo DS", "Original 2009 release", new Guid("50000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000042"), 1, "/platformLogos/wiiUVirtualConsole.png", 3, "Wii U Virtual Console", "Wii U download release", new Guid("50000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000043"), 1, "/platformLogos/WiiLogo.png", 1, "Wii", "Original 2011 release", new Guid("50000000-0000-0000-0000-000000000011") },
                    { new Guid("60000000-0000-0000-0000-000000000044"), 1, "/platformLogos/switchLogo.svg", 2, "Nintendo Switch", "Skyward Sword HD remaster", new Guid("50000000-0000-0000-0000-000000000011") },
                    { new Guid("60000000-0000-0000-0000-000000000045"), 1, "/platformLogos/3dsLogo.svg", 1, "Nintendo 3DS", "Original 2013 release", new Guid("50000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000046"), 1, "/platformLogos/3dsLogo.svg", 1, "Nintendo 3DS", "Original 2015 release", new Guid("50000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000047"), 1, "/platformLogos/wiiuLogo.png", 1, "Wii U", "Launch release family lead platform", new Guid("50000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000048"), 1, "/platformLogos/switchLogo.svg", 2, "Nintendo Switch", "Launch-day Nintendo Switch release", new Guid("50000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000049"), 2, "/platformLogos/switch2Logo.png", 2, "Nintendo Switch 2", "Nintendo Switch 2 version", new Guid("50000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000050"), 1, "/platformLogos/switchLogo.svg", 1, "Nintendo Switch", "Original 2023 release", new Guid("50000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000051"), 1, "/platformLogos/switch2Logo.png", 2, "Nintendo Switch 2", "Nintendo Switch 2 version", new Guid("50000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000052"), 1, "/platformLogos/switchLogo.svg", 1, "Nintendo Switch", "Original 2024 release", new Guid("50000000-0000-0000-0000-000000000016") },
                    { new Guid("60000000-0000-0000-0000-000000000053"), 1, "/platformLogos/gbcLogo.png", 1, "Game Boy Color", "Original 2001 release", new Guid("50000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000054"), 1, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", "3DS Virtual Console release", new Guid("50000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000055"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Game Boy app", new Guid("50000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000056"), 1, "/platformLogos/gbcLogo.png", 1, "Game Boy Color", "Original 2001 release", new Guid("50000000-0000-0000-0000-000000000018") },
                    { new Guid("60000000-0000-0000-0000-000000000057"), 1, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", "3DS Virtual Console release", new Guid("50000000-0000-0000-0000-000000000018") },
                    { new Guid("60000000-0000-0000-0000-000000000058"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Game Boy app", new Guid("50000000-0000-0000-0000-000000000018") }
                });

            migrationBuilder.InsertData(
                table: "ZeldaCatalogCollectionAppearances",
                columns: new[] { "Id", "BoxArtPath", "DisplayOrder", "Note", "PlatformImagePath", "PlatformName", "Title", "ZeldaCatalogGameId" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000001"), "/boxart/zeldaCollectorEdition.webp", 1, "Full game included on the compilation disc", "/platformLogos/gamecubeLogo.jpeg", "GameCube", "The Legend of Zelda: Collector's Edition", new Guid("50000000-0000-0000-0000-000000000001") },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "/boxart/zeldaCollectorEdition.webp", 1, "Full game included on the compilation disc", "/platformLogos/gamecubeLogo.jpeg", "GameCube", "The Legend of Zelda: Collector's Edition", new Guid("50000000-0000-0000-0000-000000000002") },
                    { new Guid("70000000-0000-0000-0000-000000000003"), "/boxart/zeldaCollectorEdition.webp", 1, "Full game included on the compilation disc", "/platformLogos/gamecubeLogo.jpeg", "GameCube", "The Legend of Zelda: Collector's Edition", new Guid("50000000-0000-0000-0000-000000000005") },
                    { new Guid("70000000-0000-0000-0000-000000000004"), "/boxart/zeldaCollectorEdition.webp", 1, "Full game included on the compilation disc", "/platformLogos/gamecubeLogo.jpeg", "GameCube", "The Legend of Zelda: Collector's Edition", new Guid("50000000-0000-0000-0000-000000000006") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEvents_AppUserId",
                table: "ActivityEvents",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEvents_CreatedAt",
                table: "ActivityEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEvents_GameId",
                table: "ActivityEvents",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OAuthProvider_OAuthSubjectId",
                table: "AppUsers",
                columns: new[] { "OAuthProvider", "OAuthSubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_AppUserId_GameId",
                table: "CollectionItems",
                columns: new[] { "AppUserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_GameId",
                table: "CollectionItems",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AppUserId",
                table: "Comments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GameId_CreatedAt",
                table: "Comments",
                columns: new[] { "GameId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GameReleases_GameId",
                table: "GameReleases",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameReleases_PlatformId",
                table: "GameReleases",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_GameReleases_RegionId",
                table: "GameReleases",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Slug",
                table: "Games",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Slug",
                table: "Platforms",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_AppUserId_GameId",
                table: "Reactions",
                columns: new[] { "AppUserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_GameId",
                table: "Reactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Code",
                table: "Regions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZeldaCatalogAvailabilities_ZeldaCatalogGameId_Kind_DisplayO~",
                table: "ZeldaCatalogAvailabilities",
                columns: new[] { "ZeldaCatalogGameId", "Kind", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ZeldaCatalogCollectionAppearances_ZeldaCatalogGameId_Displa~",
                table: "ZeldaCatalogCollectionAppearances",
                columns: new[] { "ZeldaCatalogGameId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ZeldaCatalogGames_MobyGameId",
                table: "ZeldaCatalogGames",
                column: "MobyGameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZeldaCatalogGames_Slug",
                table: "ZeldaCatalogGames",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityEvents");

            migrationBuilder.DropTable(
                name: "CollectionItems");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "GameReleases");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "ZeldaCatalogAvailabilities");

            migrationBuilder.DropTable(
                name: "ZeldaCatalogCollectionAppearances");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "ZeldaCatalogGames");
        }
    }
}
