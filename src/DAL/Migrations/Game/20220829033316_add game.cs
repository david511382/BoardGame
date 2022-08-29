using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations.Game
{
    public partial class addgame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    MaxPlayerCount = table.Column<int>(nullable: false),
                    MinPlayerCount = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInfo", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "GameInfo",
                columns: new[] { "ID", "CreateTime", "Description", "MaxPlayerCount", "MinPlayerCount", "Name" },
                values: new object[] { 1, new DateTime(2022, 8, 29, 11, 33, 16, 338, DateTimeKind.Local).AddTicks(863), "象棋遊戲", 2, 2, "軍棋" });

            migrationBuilder.InsertData(
                table: "GameInfo",
                columns: new[] { "ID", "CreateTime", "Description", "MaxPlayerCount", "MinPlayerCount", "Name" },
                values: new object[] { 2, new DateTime(2022, 8, 29, 11, 33, 16, 340, DateTimeKind.Local).AddTicks(4867), "撲克牌遊戲", 4, 2, "大老二" });

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_Name",
                table: "GameInfo",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_ID_Name",
                table: "GameInfo",
                columns: new[] { "ID", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameInfo");
        }
    }
}
