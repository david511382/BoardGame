using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MemberRepository.Migrations
{
    public partial class AddUserRegisterTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterTime",
                table: "UserInfo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterTime",
                table: "UserInfo");
        }
    }
}
