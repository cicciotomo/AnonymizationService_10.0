using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class ModifiedPropertiesIn_TransmissionToUseCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PipelineRunStatus",
                table: "AppTransmissionsToUseCases");

            migrationBuilder.AddColumn<bool>(
                name: "PipelineRunResult",
                table: "AppTransmissionsToUseCases",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PipelineRunResult",
                table: "AppTransmissionsToUseCases");

            migrationBuilder.AddColumn<string>(
                name: "PipelineRunStatus",
                table: "AppTransmissionsToUseCases",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
