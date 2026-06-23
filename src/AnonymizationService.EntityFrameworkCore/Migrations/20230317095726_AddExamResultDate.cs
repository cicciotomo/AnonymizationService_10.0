using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddExamResultDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDate",
                table: "AppLaboratoryExamResults",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultDate",
                table: "AppLaboratoryExamResults");
        }
    }
}
