using Microsoft.EntityFrameworkCore.Migrations;

namespace SirmiumCommercial.Migrations
{
    public partial class New_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupChatId",
                table: "Groups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "GroupChats",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupChatId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GroupChats");
        }
    }
}
