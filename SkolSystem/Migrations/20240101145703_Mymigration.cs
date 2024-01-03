using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkolSystem.Migrations
{
    /// <inheritdoc />
    public partial class Mymigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Klass",
                columns: table => new
                {
                    KlassID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlassNamn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klass", x => x.KlassID);
                });

            migrationBuilder.CreateTable(
                name: "Personal",
                columns: table => new
                {
                    PersonalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Befattning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PFörnamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PEfternamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnställningsDatum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lön = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal", x => x.PersonalID);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlassID = table.Column<int>(type: "int", nullable: false),
                    SFörnamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SEfternamn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_Student_Klass_KlassID",
                        column: x => x.KlassID,
                        principalTable: "Klass",
                        principalColumn: "KlassID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kurs",
                columns: table => new
                {
                    KursID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KursNamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kurs", x => x.KursID);
                    table.ForeignKey(
                        name: "FK_Kurs_Personal_PersonalID",
                        column: x => x.PersonalID,
                        principalTable: "Personal",
                        principalColumn: "PersonalID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Betyg",
                columns: table => new
                {
                    KursID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonalID = table.Column<int>(type: "int", nullable: false),
                    SlutBetyg = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Betyg", x => new { x.KursID, x.StudentID });
                    table.ForeignKey(
                        name: "FK_Betyg_Kurs_KursID",
                        column: x => x.KursID,
                        principalTable: "Kurs",
                        principalColumn: "KursID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Betyg_Personal_PersonalID",
                        column: x => x.PersonalID,
                        principalTable: "Personal",
                        principalColumn: "PersonalID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Betyg_Student_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Betyg_PersonalID",
                table: "Betyg",
                column: "PersonalID");

            migrationBuilder.CreateIndex(
                name: "IX_Betyg_StudentID",
                table: "Betyg",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Kurs_PersonalID",
                table: "Kurs",
                column: "PersonalID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_KlassID",
                table: "Student",
                column: "KlassID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Betyg");

            migrationBuilder.DropTable(
                name: "Kurs");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Personal");

            migrationBuilder.DropTable(
                name: "Klass");
        }
    }
}
