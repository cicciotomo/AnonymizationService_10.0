using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedIntensiveCareProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PipelineRunMessage",
                table: "AppIntensiveCarePatientDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PipelineRunResult",
                table: "AppIntensiveCarePatientDatas",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PipelineRunMessage",
                table: "AppIntensiveCarePatientDatas");

            migrationBuilder.DropColumn(
                name: "PipelineRunResult",
                table: "AppIntensiveCarePatientDatas");
        }
    }
}
