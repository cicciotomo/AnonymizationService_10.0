using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymizationService.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeToAbp7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppClinicalStateMachines_AppStateMachines_Id",
                table: "AppClinicalStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDbUriStateMachines_AppStateMachines_Id",
                table: "AppDbUriStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDicomStateMachines_AppStateMachines_Id",
                table: "AppDicomStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppIntensiveCareStateMachine_AppStateMachines_Id",
                table: "AppIntensiveCareStateMachine");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLaboratoryStateMachines_AppStateMachines_Id",
                table: "AppLaboratoryStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLoadPatientDataStateMachines_AppStateMachines_Id",
                table: "AppLoadPatientDataStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPathoxStateMachines_AppStateMachines_Id",
                table: "AppPathoxStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRedcapStateMachines_AppStateMachines_Id",
                table: "AppRedcapStateMachines");

            migrationBuilder.CreateTable(
                name: "AbpFeatureGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailableToHost = table.Column<bool>(type: "bit", nullable: false),
                    AllowedProviders = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MultiTenancySide = table.Column<byte>(type: "tinyint", nullable: false),
                    Providers = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StateCheckers = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureGroups_Name",
                table: "AbpFeatureGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_GroupName",
                table: "AbpFeatures",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_Name",
                table: "AbpFeatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGroups_Name",
                table: "AbpPermissionGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_GroupName",
                table: "AbpPermissions",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_Name",
                table: "AbpPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppClinicalStateMachines_AppStateMachines_Id",
                table: "AppClinicalStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppDbUriStateMachines_AppStateMachines_Id",
                table: "AppDbUriStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppDicomStateMachines_AppStateMachines_Id",
                table: "AppDicomStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppIntensiveCareStateMachine_AppStateMachines_Id",
                table: "AppIntensiveCareStateMachine",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppLaboratoryStateMachines_AppStateMachines_Id",
                table: "AppLaboratoryStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppLoadPatientDataStateMachines_AppStateMachines_Id",
                table: "AppLoadPatientDataStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppPathoxStateMachines_AppStateMachines_Id",
                table: "AppPathoxStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRedcapStateMachines_AppStateMachines_Id",
                table: "AppRedcapStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppClinicalStateMachines_AppStateMachines_Id",
                table: "AppClinicalStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDbUriStateMachines_AppStateMachines_Id",
                table: "AppDbUriStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDicomStateMachines_AppStateMachines_Id",
                table: "AppDicomStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppIntensiveCareStateMachine_AppStateMachines_Id",
                table: "AppIntensiveCareStateMachine");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLaboratoryStateMachines_AppStateMachines_Id",
                table: "AppLaboratoryStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLoadPatientDataStateMachines_AppStateMachines_Id",
                table: "AppLoadPatientDataStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPathoxStateMachines_AppStateMachines_Id",
                table: "AppPathoxStateMachines");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRedcapStateMachines_AppStateMachines_Id",
                table: "AppRedcapStateMachines");

            migrationBuilder.DropTable(
                name: "AbpFeatureGroups");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpPermissionGroups");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_AppClinicalStateMachines_AppStateMachines_Id",
                table: "AppClinicalStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDbUriStateMachines_AppStateMachines_Id",
                table: "AppDbUriStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDicomStateMachines_AppStateMachines_Id",
                table: "AppDicomStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppIntensiveCareStateMachine_AppStateMachines_Id",
                table: "AppIntensiveCareStateMachine",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLaboratoryStateMachines_AppStateMachines_Id",
                table: "AppLaboratoryStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLoadPatientDataStateMachines_AppStateMachines_Id",
                table: "AppLoadPatientDataStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPathoxStateMachines_AppStateMachines_Id",
                table: "AppPathoxStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRedcapStateMachines_AppStateMachines_Id",
                table: "AppRedcapStateMachines",
                column: "Id",
                principalTable: "AppStateMachines",
                principalColumn: "Id");
        }
    }
}
