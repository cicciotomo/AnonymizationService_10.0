using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class aggiunta_entity_redcap_ver3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MPIColumnName",
                table: "AppRedcapStudiesConfigurations",
                newName: "MpiColumnName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MpiColumnName",
                table: "AppRedcapStudiesConfigurations",
                newName: "MPIColumnName");
        }
    }
}
