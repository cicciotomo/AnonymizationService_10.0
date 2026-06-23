using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddColumnsToLaboratoryResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "AppLaboratoryExamResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamTypeDescription",
                table: "AppLaboratoryExamResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceHigh",
                table: "AppLaboratoryExamResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceLow",
                table: "AppLaboratoryExamResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                table: "AppLaboratoryExamResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AppLaboratoryExamResults",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "AppLaboratoryExamResults");

            migrationBuilder.DropColumn(
                name: "ExamTypeDescription",
                table: "AppLaboratoryExamResults");

            migrationBuilder.DropColumn(
                name: "ReferenceHigh",
                table: "AppLaboratoryExamResults");

            migrationBuilder.DropColumn(
                name: "ReferenceLow",
                table: "AppLaboratoryExamResults");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                table: "AppLaboratoryExamResults");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AppLaboratoryExamResults");
        }
    }
}
