using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedPathoxStateMachine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PathoxStartTimeIsDefault",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PathoxStateMachineShouldStart",
                table: "AppPatientUploadRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PathoxStateMachineStartTime",
                table: "AppPatientUploadRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppPathoxStateMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPathoxStateMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPathoxStateMachines_AppStateMachines_Id",
                        column: x => x.Id,
                        principalTable: "AppStateMachines",
                        principalColumn: "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPathoxStateMachines");

            migrationBuilder.DropColumn(
                name: "PathoxStartTimeIsDefault",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "PathoxStateMachineShouldStart",
                table: "AppPatientUploadRequests");

            migrationBuilder.DropColumn(
                name: "PathoxStateMachineStartTime",
                table: "AppPatientUploadRequests");
        }
    }
}
