using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class aggiunta_entity_redcap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppRedcapStudiesConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetadataFihrId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrasmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRedcapStudiesConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRedcapStudiesFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedcapStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRedcapStudiesFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRedcapStudiesFields_AppRedcapStudiesConfigurations_RedcapStudyId",
                        column: x => x.RedcapStudyId,
                        principalTable: "AppRedcapStudiesConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRedcapStudiesFields_RedcapStudyId",
                table: "AppRedcapStudiesFields",
                column: "RedcapStudyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRedcapStudiesFields");

            migrationBuilder.DropTable(
                name: "AppRedcapStudiesConfigurations");
        }
    }
}
