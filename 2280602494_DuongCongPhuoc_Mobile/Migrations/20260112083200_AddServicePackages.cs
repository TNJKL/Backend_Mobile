using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2280602494_DuongCongPhuoc_Mobile.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicePackageId",
                schema: "identity",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServicePackages",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePackages", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ServicePackages_ServicePackageId",
                schema: "identity",
                table: "Events");

            migrationBuilder.DropTable(
                name: "ServicePackages",
                schema: "identity");

            migrationBuilder.DropIndex(
                name: "IX_Events_ServicePackageId",
                schema: "identity",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ServicePackageId",
                schema: "identity",
                table: "Events");
        }
    }
}
