using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SirmiumCommercial.Migrations
{
    public partial class NotificationNew_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(nullable: true),
                    For = table.Column<string>(nullable: true),
                    ForId = table.Column<int>(nullable: false),
                    NotificationDateAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationCards",
                columns: table => new
                {
                    NotificationCardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    Msg = table.Column<string>(nullable: true),
                    NotificationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationCards", x => x.NotificationCardId);
                    table.ForeignKey(
                        name: "FK_NotificationCards_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationViews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    NotificationCardId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationViews_NotificationCards_NotificationCardId",
                        column: x => x.NotificationCardId,
                        principalTable: "NotificationCards",
                        principalColumn: "NotificationCardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationCards_NotificationId",
                table: "NotificationCards",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationViews_NotificationCardId",
                table: "NotificationViews",
                column: "NotificationCardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationViews");

            migrationBuilder.DropTable(
                name: "NotificationCards");

            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
