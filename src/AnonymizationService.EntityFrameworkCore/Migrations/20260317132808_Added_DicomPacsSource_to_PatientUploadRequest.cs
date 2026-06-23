using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class Added_DicomPacsSource_to_PatientUploadRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DicomPacsSource",
                table: "AppPatientUploadRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DicomPacsSource",
                table: "AppPatientUploadRequests");
        }
    }
}
