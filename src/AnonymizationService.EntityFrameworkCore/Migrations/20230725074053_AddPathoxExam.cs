using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddPathoxExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPathoxExams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CloudPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloudUploadDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPathoxExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPathoxExams_AppHospitalPatients_CloudPatientId",
                        column: x => x.CloudPatientId,
                        principalTable: "AppHospitalPatients",
                        principalColumn: "CloudPatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPathoxExams_CloudPatientId",
                table: "AppPathoxExams",
                column: "CloudPatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPathoxExams");
        }
    }
}
