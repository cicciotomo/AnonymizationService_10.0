using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddIntensiveCarePatientData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppIntensiveCarePatientDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloudUploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NosologicalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamEndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppIntensiveCarePatientDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppIntensiveCarePatientDatas_AppHospitalPatients_CloudPatientId",
                        column: x => x.CloudPatientId,
                        principalTable: "AppHospitalPatients",
                        principalColumn: "CloudPatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppIntensiveCarePatientDatas_CloudPatientId",
                table: "AppIntensiveCarePatientDatas",
                column: "CloudPatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppIntensiveCarePatientDatas");
        }
    }
}
