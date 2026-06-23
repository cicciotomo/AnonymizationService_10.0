using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class Added_TransmissionToUseCaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTransmissionsToUseCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RedcapStudyConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UseCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PipelineRunStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PipelineRunMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTransmissionsToUseCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTransmissionsToUseCases_AppRedcapStudiesConfigurations_RedcapStudyConfigurationId",
                        column: x => x.RedcapStudyConfigurationId,
                        principalTable: "AppRedcapStudiesConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTransmissionsToUseCases_RedcapStudyConfigurationId",
                table: "AppTransmissionsToUseCases",
                column: "RedcapStudyConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTransmissionsToUseCases");
        }
    }
}
