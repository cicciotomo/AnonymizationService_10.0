using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedIntensiveCareParamenters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IntensiveCareStartTimeIsDefault",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IntensiveCareStateMachineNosologicalCode",
                table: "AppPatientUploadRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IntensiveCareStateMachineShouldStart",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IntensiveCareStateMachineStartTime",
                table: "AppPatientUploadRequests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntensiveCareStartTimeIsDefault",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "IntensiveCareStateMachineNosologicalCode",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "IntensiveCareStateMachineShouldStart",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "IntensiveCareStateMachineStartTime",
                table: "AppPatientUploadRequests");
        }
    }
}
