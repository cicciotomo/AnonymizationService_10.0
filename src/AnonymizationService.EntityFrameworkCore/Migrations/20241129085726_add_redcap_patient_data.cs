using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class add_redcap_patient_data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRedcapStudiesFields");

            migrationBuilder.CreateTable(
                name: "AppRedcapPatientData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RedcapStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RedcapRecordId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloudPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloudUploadDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRedcapPatientData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRedcapPatientData_AppRedcapStudiesConfigurations_RedcapStudyId",
                        column: x => x.RedcapStudyId,
                        principalTable: "AppRedcapStudiesConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRedcapPatientData_RedcapStudyId",
                table: "AppRedcapPatientData",
                column: "RedcapStudyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRedcapPatientData");

            migrationBuilder.CreateTable(
                name: "AppRedcapStudiesFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedcapStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRedcapStudiesFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRedcapStudiesFields_AppRedcapStudiesConfigurations_RedcapStudyId",
                        column: x => x.RedcapStudyId,
                        principalTable: "AppRedcapStudiesConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRedcapStudiesFields_RedcapStudyId",
                table: "AppRedcapStudiesFields",
                column: "RedcapStudyId");
        }
    }
}
