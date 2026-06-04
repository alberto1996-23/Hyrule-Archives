using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddZeldaCatalogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZeldaCatalogGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MobyGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BoxArtPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeldaCatalogGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZeldaCatalogAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ZeldaCatalogGameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeldaCatalogAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZeldaCatalogAvailabilities_ZeldaCatalogGames_ZeldaCatalogGameId",
                        column: x => x.ZeldaCatalogGameId,
                        principalTable: "ZeldaCatalogGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZeldaCatalogCollectionAppearances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ZeldaCatalogGameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BoxArtPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PlatformName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PlatformImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeldaCatalogCollectionAppearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZeldaCatalogCollectionAppearances_ZeldaCatalogGames_ZeldaCatalogGameId",
                        column: x => x.ZeldaCatalogGameId,
                        principalTable: "ZeldaCatalogGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ZeldaCatalogGames",
                columns: new[] { "Id", "BoxArtPath", "MobyGameId", "Slug", "Title" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "/boxart/zeldanes.jpeg", 3393, "the-legend-of-zelda", "The Legend of Zelda" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "/boxart/zelda2.jpg", 7296, "zelda-ii-the-adventure-of-link", "Zelda II: The Adventure of Link" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "/boxart/aLinktoThePast.jpg", 6608, "a-link-to-the-past", "The Legend of Zelda: A Link to the Past" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "/boxart/linksAwakening.png", 4267, "links-awakening", "The Legend of Zelda: Link's Awakening" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "/boxart/ocarinaOfTime.jpg", 3549, "ocarina-of-time", "The Legend of Zelda: Ocarina of Time" },
                    { new Guid("50000000-0000-0000-0000-000000000006"), "/boxart/majorasMask.jpg", 3550, "majoras-mask", "The Legend of Zelda: Majora's Mask" },
                    { new Guid("50000000-0000-0000-0000-000000000007"), "/boxart/windWaker.webp", 8725, "the-wind-waker", "The Legend of Zelda: The Wind Waker" },
                    { new Guid("50000000-0000-0000-0000-000000000008"), "/boxart/twilightPrincess.jpg", 25103, "twilight-princess", "The Legend of Zelda: Twilight Princess" },
                    { new Guid("50000000-0000-0000-0000-000000000009"), "/boxart/phantomHourglass.jpg", 30522, "phantom-hourglass", "The Legend of Zelda: Phantom Hourglass" },
                    { new Guid("50000000-0000-0000-0000-000000000010"), "/boxart/spiritTracks.jpg", 44059, "spirit-tracks", "The Legend of Zelda: Spirit Tracks" },
                    { new Guid("50000000-0000-0000-0000-000000000011"), "/boxart/skywardSword.webp", 53708, "skyward-sword", "The Legend of Zelda: Skyward Sword" },
                    { new Guid("50000000-0000-0000-0000-000000000012"), "/boxart/aLinkbetweenWorlds.jpg", 62785, "a-link-between-worlds", "The Legend of Zelda: A Link Between Worlds" },
                    { new Guid("50000000-0000-0000-0000-000000000013"), "/boxart/triforceHeroes.avif", 75907, "tri-force-heroes", "The Legend of Zelda: Tri Force Heroes" },
                    { new Guid("50000000-0000-0000-0000-000000000014"), "/boxart/breathOfTheWild.jpg", 84595, "breath-of-the-wild", "The Legend of Zelda: Breath of the Wild" },
                    { new Guid("50000000-0000-0000-0000-000000000015"), "/boxart/tearsOfTheKingdom.jpg", 203119, "tears-of-the-kingdom", "The Legend of Zelda: Tears of the Kingdom" },
                    { new Guid("50000000-0000-0000-0000-000000000016"), "/boxart/echoesOfWisdom.avif", 231129, "echoes-of-wisdom", "The Legend of Zelda: Echoes of Wisdom" }
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
                    { new Guid("60000000-0000-0000-0000-000000000052"), 1, "/platformLogos/switchLogo.svg", 1, "Nintendo Switch", "Original 2024 release", new Guid("50000000-0000-0000-0000-000000000016") }
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
                name: "IX_ZeldaCatalogAvailabilities_ZeldaCatalogGameId_Kind_DisplayOrder",
                table: "ZeldaCatalogAvailabilities",
                columns: new[] { "ZeldaCatalogGameId", "Kind", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ZeldaCatalogCollectionAppearances_ZeldaCatalogGameId_DisplayOrder",
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
                name: "ZeldaCatalogAvailabilities");

            migrationBuilder.DropTable(
                name: "ZeldaCatalogCollectionAppearances");

            migrationBuilder.DropTable(
                name: "ZeldaCatalogGames");
        }
    }
}
