using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class aggiunta_entity_redcap_ver2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "AppRedcapStudiesConfigurations",
                newName: "RedcapToken");

            migrationBuilder.AddColumn<string>(
                name: "MPIColumnName",
                table: "AppRedcapStudiesConfigurations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MPIColumnName",
                table: "AppRedcapStudiesConfigurations");

            migrationBuilder.RenameColumn(
                name: "RedcapToken",
                table: "AppRedcapStudiesConfigurations",
                newName: "AccessToken");
        }
    }
}
