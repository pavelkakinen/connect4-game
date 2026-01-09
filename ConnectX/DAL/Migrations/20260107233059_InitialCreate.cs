using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SavedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BoardWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    WinCond = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardType = table.Column<int>(type: "INTEGER", nullable: false),
                    Player1Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Player2Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsNextMoveByRed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Board = table.Column<string>(type: "TEXT", nullable: false),
                    P1Type = table.Column<int>(type: "INTEGER", nullable: false),
                    P2Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
