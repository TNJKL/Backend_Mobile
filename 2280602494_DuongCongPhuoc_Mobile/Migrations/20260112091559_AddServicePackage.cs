using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2280602494_DuongCongPhuoc_Mobile.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ServicePackages_ServicePackageId",
                schema: "identity",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ServicePackageId",
                schema: "identity",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "identity",
                table: "ServicePackages");

            migrationBuilder.DropColumn(
                name: "Features",
                schema: "identity",
                table: "ServicePackages");

            migrationBuilder.DropColumn(
                name: "ServicePackageId",
                schema: "identity",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "identity",
                table: "ServicePackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "identity",
                table: "ServicePackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "identity",
                table: "ServicePackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GlobalMenuCatalogs",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalMenuCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicePackageItems",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicePackageId = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    CustomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePackageItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicePackageItems_ServicePackages_ServicePackageId",
                        column: x => x.ServicePackageId,
                        principalSchema: "identity",
                        principalTable: "ServicePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeddingTaskCategories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeddingTaskCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackageItems_ServicePackageId",
                schema: "identity",
                table: "ServicePackageItems",
                column: "ServicePackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalMenuCatalogs",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ServicePackageItems",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "WeddingTaskCategories",
                schema: "identity");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "identity",
                table: "ServicePackages");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "identity",
                table: "ServicePackages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "identity",
                table: "ServicePackages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "identity",
                table: "ServicePackages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Features",
                schema: "identity",
                table: "ServicePackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicePackageId",
                schema: "identity",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ServicePackageId",
                schema: "identity",
                table: "Events",
                column: "ServicePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ServicePackages_ServicePackageId",
                schema: "identity",
                table: "Events",
                column: "ServicePackageId",
                principalSchema: "identity",
                principalTable: "ServicePackages",
                principalColumn: "Id");
        }
    }
}
