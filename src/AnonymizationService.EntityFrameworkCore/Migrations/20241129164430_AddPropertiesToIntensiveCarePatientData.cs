using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddPropertiesToIntensiveCarePatientData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PipelineId",
                table: "AppIntensiveCarePatientDatas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PipelineRunCreationTime",
                table: "AppIntensiveCarePatientDatas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PipelineRunEndTime",
                table: "AppIntensiveCarePatientDatas",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PipelineId",
                table: "AppIntensiveCarePatientDatas");

            migrationBuilder.DropColumn(
                name: "PipelineRunCreationTime",
                table: "AppIntensiveCarePatientDatas");

            migrationBuilder.DropColumn(
                name: "PipelineRunEndTime",
                table: "AppIntensiveCarePatientDatas");
        }
    }
}
