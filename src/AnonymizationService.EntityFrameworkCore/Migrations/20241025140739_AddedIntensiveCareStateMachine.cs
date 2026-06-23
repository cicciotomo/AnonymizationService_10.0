using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    public partial class AddedIntensiveCareStateMachine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppIntensiveCareStateMachine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppIntensiveCareStateMachine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppIntensiveCareStateMachine_AppStateMachines_Id",
                        column: x => x.Id,
                        principalTable: "AppStateMachines",
                        principalColumn: "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppIntensiveCareStateMachine");
        }
    }
}
