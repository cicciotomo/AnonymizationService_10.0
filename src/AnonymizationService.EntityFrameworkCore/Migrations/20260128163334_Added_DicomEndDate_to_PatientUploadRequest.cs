using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class Added_DicomEndDate_to_PatientUploadRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DicomStateMachineEndTime",
                table: "AppPatientUploadRequests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DicomStateMachineEndTime",
                table: "AppPatientUploadRequests");
        }
    }
}
