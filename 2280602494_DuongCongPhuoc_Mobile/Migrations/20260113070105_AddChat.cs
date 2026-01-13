using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2280602494_DuongCongPhuoc_Mobile.Migrations
{
    /// <inheritdoc />
    public partial class AddChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMilestones",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventVendorId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMilestones_EventVendors_EventVendorId",
                        column: x => x.EventVendorId,
                        principalSchema: "identity",
                        principalTable: "EventVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackageItems_ReferenceId",
                schema: "identity",
                table: "ServicePackageItems",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReceiverId",
                schema: "identity",
                table: "ChatMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                schema: "identity",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMilestones_EventVendorId",
                schema: "identity",
                table: "PaymentMilestones",
                column: "EventVendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePackageItems_GlobalMenuItems_ReferenceId",
                schema: "identity",
                table: "ServicePackageItems",
                column: "ReferenceId",
                principalSchema: "identity",
                principalTable: "GlobalMenuItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePackageItems_GlobalMenuItems_ReferenceId",
                schema: "identity",
                table: "ServicePackageItems");

            migrationBuilder.DropTable(
                name: "ChatMessages",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "PaymentMilestones",
                schema: "identity");

            migrationBuilder.DropIndex(
                name: "IX_ServicePackageItems_ReferenceId",
                schema: "identity",
                table: "ServicePackageItems");
        }
    }
}
