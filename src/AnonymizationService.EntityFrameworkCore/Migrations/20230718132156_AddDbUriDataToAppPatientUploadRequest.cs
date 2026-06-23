using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddDbUriDataToAppPatientUploadRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DbUriStartTimeIsDefault",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DbUriStateMachineShouldStart",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DbUriStateMachineStartTime",
                table: "AppPatientUploadRequests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DbUriStartTimeIsDefault",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "DbUriStateMachineShouldStart",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "DbUriStateMachineStartTime",
                table: "AppPatientUploadRequests");
        }
    }
}
