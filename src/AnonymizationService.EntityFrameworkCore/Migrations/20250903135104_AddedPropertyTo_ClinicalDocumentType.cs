using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedPropertyTo_ClinicalDocumentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Enabled",
                table: "AppClinicalDocumentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "AppClinicalDocumentTypes");
        }
    }
}
