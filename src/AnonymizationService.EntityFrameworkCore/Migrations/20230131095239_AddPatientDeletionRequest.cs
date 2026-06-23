using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddPatientDeletionRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDeletionRequestDate",
                table: "AppHospitalPatients",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDeletionRequestDate",
                table: "AppHospitalPatients");
        }
    }
}
