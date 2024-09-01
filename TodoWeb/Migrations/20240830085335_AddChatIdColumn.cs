using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoWeb.Migrations
{
    public partial class AddChatIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IdProject",
                table: "Messages",
                column: "IdProject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_IdProject",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Messages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                columns: new[] { "IdProject", "IdUser" });
        }
    }
}
