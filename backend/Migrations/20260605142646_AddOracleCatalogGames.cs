using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOracleCatalogGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ZeldaCatalogGames",
                columns: new[] { "Id", "BoxArtPath", "MobyGameId", "Slug", "Title" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000017"), "/boxart/oracleOfSeasons.webp", 4269, "oracle-of-seasons", "The Legend of Zelda: Oracle of Seasons" },
                    { new Guid("50000000-0000-0000-0000-000000000018"), "/boxart/oracleOfAges.webp", 4270, "oracle-of-ages", "The Legend of Zelda: Oracle of Ages" }
                });

            migrationBuilder.InsertData(
                table: "ZeldaCatalogAvailabilities",
                columns: new[] { "Id", "DisplayOrder", "ImagePath", "Kind", "Name", "Note", "ZeldaCatalogGameId" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000053"), 1, "/platformLogos/gbcLogo.png", 1, "Game Boy Color", "Original 2001 release", new Guid("50000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000054"), 1, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", "3DS Virtual Console release", new Guid("50000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000055"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Game Boy app", new Guid("50000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000056"), 1, "/platformLogos/gbcLogo.png", 1, "Game Boy Color", "Original 2001 release", new Guid("50000000-0000-0000-0000-000000000018") },
                    { new Guid("60000000-0000-0000-0000-000000000057"), 1, "/platformLogos/3dsVirtualConsole.png", 3, "Nintendo 3DS Virtual Console", "3DS Virtual Console release", new Guid("50000000-0000-0000-0000-000000000018") },
                    { new Guid("60000000-0000-0000-0000-000000000058"), 1, "/platformLogos/nintendoSwitchOnlineLogo.png", 4, "Nintendo Switch Online", "Available in the Game Boy app", new Guid("50000000-0000-0000-0000-000000000018") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ZeldaCatalogAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000053"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000054"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000055"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000056"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000057"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogAvailabilities",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000058"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000018"));
        }
    }
}
