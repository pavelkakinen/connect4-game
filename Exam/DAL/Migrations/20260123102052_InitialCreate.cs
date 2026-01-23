using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanceStyles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanceStyles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SizeSquareMeters = table.Column<int>(type: "INTEGER", nullable: false),
                    HasPoles = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasAerialRigging = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxCapacity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    PackageType = table.Column<int>(type: "INTEGER", nullable: false),
                    DanceStyleId = table.Column<int>(type: "INTEGER", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RemainingClasses = table.Column<int>(type: "INTEGER", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_DanceStyles_DanceStyleId",
                        column: x => x.DanceStyleId,
                        principalTable: "DanceStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrialUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DanceStyleId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrialDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrialUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrialUsages_DanceStyles_DanceStyleId",
                        column: x => x.DanceStyleId,
                        principalTable: "DanceStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrialUsages_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanceClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DanceStyleId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudioId = table.Column<int>(type: "INTEGER", nullable: false),
                    InstructorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    MaxStudents = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanceClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanceClasses_DanceStyles_DanceStyleId",
                        column: x => x.DanceStyleId,
                        principalTable: "DanceStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanceClasses_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DanceClassId = table.Column<int>(type: "INTEGER", nullable: false),
                    PackageId = table.Column<int>(type: "INTEGER", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsTrial = table.Column<bool>(type: "INTEGER", nullable: false),
                    InvitedByStudentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Attended = table.Column<bool>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_DanceClasses_DanceClassId",
                        column: x => x.DanceClassId,
                        principalTable: "DanceClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Students_InvitedByStudentId",
                        column: x => x.InvitedByStudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DanceStyles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Ballet" },
                    { 2, "Contemporary" },
                    { 3, "Jazz" },
                    { 4, "Hip-hop" },
                    { 5, "Salsa" },
                    { 6, "Bachata" },
                    { 7, "Tango" },
                    { 8, "Ballroom" },
                    { 9, "Pole Fitness" },
                    { 10, "Aerial Silks" },
                    { 11, "Breakdancing" },
                    { 12, "K-pop" }
                });

            migrationBuilder.InsertData(
                table: "Studios",
                columns: new[] { "Id", "HasAerialRigging", "HasPoles", "MaxCapacity", "Name", "SizeSquareMeters" },
                values: new object[,]
                {
                    { 1, false, false, 25, "Studio A", 120 },
                    { 2, false, false, 18, "Studio B", 80 },
                    { 3, false, true, 12, "Studio C", 60 },
                    { 4, true, false, 10, "Studio D", 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DanceClassId",
                table: "Bookings",
                column: "DanceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_InvitedByStudentId",
                table: "Bookings",
                column: "InvitedByStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PackageId",
                table: "Bookings",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StudentId",
                table: "Bookings",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceClasses_DanceStyleId",
                table: "DanceClasses",
                column: "DanceStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceClasses_StudioId",
                table: "DanceClasses",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DanceStyleId",
                table: "Packages",
                column: "DanceStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_StudentId",
                table: "Packages",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrialUsages_DanceStyleId",
                table: "TrialUsages",
                column: "DanceStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_TrialUsages_StudentId_DanceStyleId",
                table: "TrialUsages",
                columns: new[] { "StudentId", "DanceStyleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "TrialUsages");

            migrationBuilder.DropTable(
                name: "DanceClasses");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Studios");

            migrationBuilder.DropTable(
                name: "DanceStyles");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
