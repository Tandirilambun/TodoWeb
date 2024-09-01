using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoWeb.Migrations
{
    public partial class FixUuidGenerateChatId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ChatId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ChatId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");
        }
    }
}
