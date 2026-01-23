using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDanceStyleDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DanceStyles",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanceStyles",
                keyColumn: "Id",
                keyValue: 12,
                column: "Description",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DanceStyles");
        }
    }
}
