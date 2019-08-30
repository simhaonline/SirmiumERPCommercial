using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SirmiumCommercial.Migrations
{
    public partial class Initial11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoURL",
                table: "Representations");

            migrationBuilder.DropColumn(
                name: "VideoURL",
                table: "Presentations");

            migrationBuilder.DropColumn(
                name: "VideoURL",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "Representations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "Presentations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "Courses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    For = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    Views = table.Column<int>(nullable: false),
                    VideoPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Representations");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Presentations");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "VideoURL",
                table: "Representations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoURL",
                table: "Presentations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoURL",
                table: "Courses",
                nullable: true);
        }
    }
}
