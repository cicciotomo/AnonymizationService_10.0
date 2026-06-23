using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedPropertiesTo_TransmissionToUseCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PipelineRunCreationTime",
                table: "AppTransmissionsToUseCases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PipelineRunEndTime",
                table: "AppTransmissionsToUseCases",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PipelineRunCreationTime",
                table: "AppTransmissionsToUseCases");

            migrationBuilder.DropColumn(
                name: "PipelineRunEndTime",
                table: "AppTransmissionsToUseCases");
        }
    }
}
