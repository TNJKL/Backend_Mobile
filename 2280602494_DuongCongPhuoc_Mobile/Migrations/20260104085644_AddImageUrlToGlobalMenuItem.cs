using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2280602494_DuongCongPhuoc_Mobile.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToGlobalMenuItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "identity",
                table: "GlobalMenuItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "identity",
                table: "GlobalMenuItems");
        }
    }
}
