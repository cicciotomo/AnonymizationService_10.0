using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddRedcapStateMachine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RedcapStartTimeIsDefault",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RedcapStateMachineRecordId",
                table: "AppPatientUploadRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RedcapStateMachineRedcapStudyConfigurationId",
                table: "AppPatientUploadRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RedcapStateMachineShouldStart",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RedcapStateMachineStartTime",
                table: "AppPatientUploadRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastIntensiveCareDataUpdateDate",
                table: "AppHospitalPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRedcapDataUpdateDate",
                table: "AppHospitalPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppRedcapStateMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRedcapStateMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRedcapStateMachines_AppStateMachines_Id",
                        column: x => x.Id,
                        principalTable: "AppStateMachines",
                        principalColumn: "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRedcapStateMachines");

            migrationBuilder.DropColumn(
                name: "RedcapStartTimeIsDefault",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "RedcapStateMachineRecordId",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "RedcapStateMachineRedcapStudyConfigurationId",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "RedcapStateMachineShouldStart",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "RedcapStateMachineStartTime",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "LastIntensiveCareDataUpdateDate",
                table: "AppHospitalPatients");

            migrationBuilder.DropColumn(
                name: "LastRedcapDataUpdateDate",
                table: "AppHospitalPatients");
        }
    }
}
