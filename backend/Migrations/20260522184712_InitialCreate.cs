using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OAuthProvider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OAuthSubjectId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    IsSimulated = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalApiId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameReleases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlatformId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OAuthProvider_OAuthSubjectId",
                table: "AppUsers",
                columns: new[] { "OAuthProvider", "OAuthSubjectId" },
                unique: true);

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
                name: "IX_Regions_Code",
                table: "Regions",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "GameReleases");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
