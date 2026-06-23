using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedDbUriDataEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDbUriDataUpdateDate",
                table: "AppHospitalPatients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppDbUriEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CloudPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloudUploadDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDbUriEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDbUriEvents_AppHospitalPatients_CloudPatientId",
                        column: x => x.CloudPatientId,
                        principalTable: "AppHospitalPatients",
                        principalColumn: "CloudPatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppDbUriFUpItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CloudPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloudUploadDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDbUriFUpItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDbUriFUpItems_AppHospitalPatients_CloudPatientId",
                        column: x => x.CloudPatientId,
                        principalTable: "AppHospitalPatients",
                        principalColumn: "CloudPatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDbUriEvents_CloudPatientId",
                table: "AppDbUriEvents",
                column: "CloudPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDbUriFUpItems_CloudPatientId",
                table: "AppDbUriFUpItems",
                column: "CloudPatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDbUriEvents");

            migrationBuilder.DropTable(
                name: "AppDbUriFUpItems");

            migrationBuilder.DropColumn(
                name: "LastDbUriDataUpdateDate",
                table: "AppHospitalPatients");
        }
    }
}
