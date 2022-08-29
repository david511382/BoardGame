using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations.Member
{
    public partial class addmember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 10, nullable: true),
                    Username = table.Column<string>(maxLength: 16, nullable: false),
                    Password = table.Column<string>(maxLength: 64, nullable: false),
                    RegisterTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "ID", "Name", "Password", "RegisterTime", "Username" },
                values: new object[,]
                {
                    { 1, "tester", "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", new DateTime(2022, 8, 29, 11, 32, 43, 700, DateTimeKind.Local).AddTicks(8303), "test" },
                    { 2, "tester1", "1b4f0e9851971998e732078544c96b36c3d01cedf7caa332359d6f1d83567014", new DateTime(2022, 8, 29, 11, 32, 43, 701, DateTimeKind.Local).AddTicks(3056), "test1" },
                    { 3, "tester2", "60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752", new DateTime(2022, 8, 29, 11, 32, 43, 701, DateTimeKind.Local).AddTicks(3057), "test2" },
                    { 4, "tester3", "fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13", new DateTime(2022, 8, 29, 11, 32, 43, 701, DateTimeKind.Local).AddTicks(3058), "test3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Username",
                table: "UserInfo",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Username_Password",
                table: "UserInfo",
                columns: new[] { "Username", "Password" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
