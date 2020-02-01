using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameRespository.Migrations
{
    public partial class TestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GameInfo",
                columns: new[] { "ID", "CreateTime", "Description", "MaxPlayerCount", "MinPlayerCount", "Name" },
                values: new object[] { 1, new DateTime(2020, 2, 1, 12, 4, 8, 594, DateTimeKind.Local).AddTicks(5866), "象棋遊戲", 2, 2, "軍棋" });

            migrationBuilder.InsertData(
                table: "GameInfo",
                columns: new[] { "ID", "CreateTime", "Description", "MaxPlayerCount", "MinPlayerCount", "Name" },
                values: new object[] { 2, new DateTime(2020, 2, 1, 12, 4, 8, 595, DateTimeKind.Local).AddTicks(1245), "撲克牌遊戲", 4, 2, "大老二" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GameInfo",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GameInfo",
                keyColumn: "ID",
                keyValue: 2);
        }
    }
}
