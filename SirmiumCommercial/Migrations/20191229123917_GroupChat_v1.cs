using Microsoft.EntityFrameworkCore.Migrations;

namespace SirmiumCommercial.Migrations
{
    public partial class GroupChat_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatMessages_GroupChats_GroupChatChatId",
                table: "GroupChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatUsers_GroupChats_GroupChatChatId",
                table: "GroupChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessageViews_GroupChatMessages_GroupChatMessageMessageId",
                table: "GroupMessageViews");

            migrationBuilder.DropIndex(
                name: "IX_GroupMessageViews_GroupChatMessageMessageId",
                table: "GroupMessageViews");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatUsers_GroupChatChatId",
                table: "GroupChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatMessages_GroupChatChatId",
                table: "GroupChatMessages");

            migrationBuilder.DropColumn(
                name: "GroupChatMessageMessageId",
                table: "GroupMessageViews");

            migrationBuilder.DropColumn(
                name: "GroupChatChatId",
                table: "GroupChatUsers");

            migrationBuilder.DropColumn(
                name: "GroupChatChatId",
                table: "GroupChatMessages");

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "GroupMessageViews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupChatId",
                table: "GroupChatUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupChatId",
                table: "GroupChatMessages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessageViews_MessageId",
                table: "GroupMessageViews",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatUsers_GroupChatId",
                table: "GroupChatUsers",
                column: "GroupChatId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupChatId",
                table: "GroupChatMessages",
                column: "GroupChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatMessages_GroupChats_GroupChatId",
                table: "GroupChatMessages",
                column: "GroupChatId",
                principalTable: "GroupChats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatUsers_GroupChats_GroupChatId",
                table: "GroupChatUsers",
                column: "GroupChatId",
                principalTable: "GroupChats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessageViews_GroupChatMessages_MessageId",
                table: "GroupMessageViews",
                column: "MessageId",
                principalTable: "GroupChatMessages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatMessages_GroupChats_GroupChatId",
                table: "GroupChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatUsers_GroupChats_GroupChatId",
                table: "GroupChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessageViews_GroupChatMessages_MessageId",
                table: "GroupMessageViews");

            migrationBuilder.DropIndex(
                name: "IX_GroupMessageViews_MessageId",
                table: "GroupMessageViews");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatUsers_GroupChatId",
                table: "GroupChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatMessages_GroupChatId",
                table: "GroupChatMessages");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "GroupMessageViews");

            migrationBuilder.DropColumn(
                name: "GroupChatId",
                table: "GroupChatUsers");

            migrationBuilder.DropColumn(
                name: "GroupChatId",
                table: "GroupChatMessages");

            migrationBuilder.AddColumn<int>(
                name: "GroupChatMessageMessageId",
                table: "GroupMessageViews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupChatChatId",
                table: "GroupChatUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupChatChatId",
                table: "GroupChatMessages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessageViews_GroupChatMessageMessageId",
                table: "GroupMessageViews",
                column: "GroupChatMessageMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatUsers_GroupChatChatId",
                table: "GroupChatUsers",
                column: "GroupChatChatId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupChatChatId",
                table: "GroupChatMessages",
                column: "GroupChatChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatMessages_GroupChats_GroupChatChatId",
                table: "GroupChatMessages",
                column: "GroupChatChatId",
                principalTable: "GroupChats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatUsers_GroupChats_GroupChatChatId",
                table: "GroupChatUsers",
                column: "GroupChatChatId",
                principalTable: "GroupChats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessageViews_GroupChatMessages_GroupChatMessageMessageId",
                table: "GroupMessageViews",
                column: "GroupChatMessageMessageId",
                principalTable: "GroupChatMessages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
