using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DanceClasses",
                columns: new[] { "Id", "DanceStyleId", "DayOfWeek", "EndTime", "InstructorName", "Level", "MaxStudents", "StartTime", "StudioId" },
                values: new object[,]
                {
                    { 1, 1, 1, new TimeSpan(0, 19, 0, 0, 0), "Maria Santos", 0, 20, new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 2, 5, 1, new TimeSpan(0, 19, 0, 0, 0), "Carlos Rivera", 1, 15, new TimeSpan(0, 18, 0, 0, 0), 2 },
                    { 3, 9, 1, new TimeSpan(0, 20, 0, 0, 0), "Emma Wilson", 0, 10, new TimeSpan(0, 19, 0, 0, 0), 3 },
                    { 4, 4, 1, new TimeSpan(0, 21, 0, 0, 0), "Jake Thompson", 2, 18, new TimeSpan(0, 20, 0, 0, 0), 1 },
                    { 5, 2, 2, new TimeSpan(0, 19, 0, 0, 0), "Sofia Chen", 1, 20, new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 6, 6, 2, new TimeSpan(0, 20, 0, 0, 0), "Carlos Rivera", 0, 15, new TimeSpan(0, 19, 0, 0, 0), 2 },
                    { 7, 10, 2, new TimeSpan(0, 19, 30, 0, 0), "Luna Park", 1, 8, new TimeSpan(0, 18, 0, 0, 0), 4 },
                    { 8, 3, 3, new TimeSpan(0, 19, 0, 0, 0), "Nina Johnson", 0, 22, new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 9, 7, 3, new TimeSpan(0, 20, 0, 0, 0), "Marco Rossi", 2, 14, new TimeSpan(0, 19, 0, 0, 0), 2 },
                    { 10, 12, 3, new TimeSpan(0, 21, 0, 0, 0), "Min-Jun Kim", 1, 20, new TimeSpan(0, 20, 0, 0, 0), 1 },
                    { 11, 1, 4, new TimeSpan(0, 19, 0, 0, 0), "Maria Santos", 1, 18, new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 12, 8, 4, new TimeSpan(0, 19, 0, 0, 0), "Victoria Adams", 0, 16, new TimeSpan(0, 18, 0, 0, 0), 2 },
                    { 13, 9, 4, new TimeSpan(0, 20, 0, 0, 0), "Emma Wilson", 1, 10, new TimeSpan(0, 19, 0, 0, 0), 3 },
                    { 14, 5, 5, new TimeSpan(0, 19, 0, 0, 0), "Carlos Rivera", 2, 16, new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 15, 11, 5, new TimeSpan(0, 20, 0, 0, 0), "DJ Marcus", 0, 20, new TimeSpan(0, 19, 0, 0, 0), 1 },
                    { 16, 4, 5, new TimeSpan(0, 21, 0, 0, 0), "Jake Thompson", 3, 12, new TimeSpan(0, 20, 0, 0, 0), 2 },
                    { 17, 6, 6, new TimeSpan(0, 19, 0, 0, 0), "Carlos Rivera", 1, 20, new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 18, 10, 6, new TimeSpan(0, 19, 30, 0, 0), "Luna Park", 2, 8, new TimeSpan(0, 18, 0, 0, 0), 4 },
                    { 19, 2, 6, new TimeSpan(0, 21, 0, 0, 0), "Sofia Chen", 3, 15, new TimeSpan(0, 19, 30, 0, 0), 1 },
                    { 20, 1, 0, new TimeSpan(0, 11, 0, 0, 0), "Maria Santos", 0, 25, new TimeSpan(0, 10, 0, 0, 0), 1 },
                    { 21, 3, 0, new TimeSpan(0, 12, 0, 0, 0), "Nina Johnson", 1, 18, new TimeSpan(0, 11, 0, 0, 0), 2 },
                    { 22, 12, 0, new TimeSpan(0, 13, 0, 0, 0), "Min-Jun Kim", 0, 22, new TimeSpan(0, 12, 0, 0, 0), 1 },
                    { 23, 9, 0, new TimeSpan(0, 13, 30, 0, 0), "Emma Wilson", 2, 8, new TimeSpan(0, 12, 0, 0, 0), 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "DanceClasses",
                keyColumn: "Id",
                keyValue: 23);
        }
    }
}
