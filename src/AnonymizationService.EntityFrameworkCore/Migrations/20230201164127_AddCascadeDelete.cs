using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppLaboratoryExamResults_AppLaboratoryExams_LaboratoryExamId",
                table: "AppLaboratoryExamResults");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLaboratoryExamResults_AppLaboratoryExams_LaboratoryExamId",
                table: "AppLaboratoryExamResults",
                column: "LaboratoryExamId",
                principalTable: "AppLaboratoryExams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppLaboratoryExamResults_AppLaboratoryExams_LaboratoryExamId",
                table: "AppLaboratoryExamResults");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLaboratoryExamResults_AppLaboratoryExams_LaboratoryExamId",
                table: "AppLaboratoryExamResults",
                column: "LaboratoryExamId",
                principalTable: "AppLaboratoryExams",
                principalColumn: "Id");
        }
    }
}
