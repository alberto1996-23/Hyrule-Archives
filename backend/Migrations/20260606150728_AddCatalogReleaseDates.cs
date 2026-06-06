using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogReleaseDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalReleaseDate",
                table: "ZeldaCatalogGames",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "OriginalReleaseDate",
                value: new DateTime(1986, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "OriginalReleaseDate",
                value: new DateTime(1987, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "OriginalReleaseDate",
                value: new DateTime(1991, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"),
                column: "OriginalReleaseDate",
                value: new DateTime(1993, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"),
                column: "OriginalReleaseDate",
                value: new DateTime(1998, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"),
                column: "OriginalReleaseDate",
                value: new DateTime(2000, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000007"),
                column: "OriginalReleaseDate",
                value: new DateTime(2002, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000008"),
                column: "OriginalReleaseDate",
                value: new DateTime(2006, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000009"),
                column: "OriginalReleaseDate",
                value: new DateTime(2007, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000010"),
                column: "OriginalReleaseDate",
                value: new DateTime(2009, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000011"),
                column: "OriginalReleaseDate",
                value: new DateTime(2011, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000012"),
                column: "OriginalReleaseDate",
                value: new DateTime(2013, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000013"),
                column: "OriginalReleaseDate",
                value: new DateTime(2015, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000014"),
                column: "OriginalReleaseDate",
                value: new DateTime(2017, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000015"),
                column: "OriginalReleaseDate",
                value: new DateTime(2023, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000016"),
                column: "OriginalReleaseDate",
                value: new DateTime(2024, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000017"),
                column: "OriginalReleaseDate",
                value: new DateTime(2001, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ZeldaCatalogGames",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000018"),
                column: "OriginalReleaseDate",
                value: new DateTime(2001, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalReleaseDate",
                table: "ZeldaCatalogGames");
        }
    }
}
