using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MemberRepository.Migrations
{
    public partial class TestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "ID", "Name", "Password", "RegisterTime", "Username" },
                values: new object[,]
                {
                    { 1, "tester", "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", new DateTime(2020, 2, 1, 12, 37, 59, 607, DateTimeKind.Local).AddTicks(2125), "test" },
                    { 2, "tester1", "1b4f0e9851971998e732078544c96b36c3d01cedf7caa332359d6f1d83567014", new DateTime(2020, 2, 1, 12, 37, 59, 607, DateTimeKind.Local).AddTicks(8023), "test1" },
                    { 3, "tester2", "60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752", new DateTime(2020, 2, 1, 12, 37, 59, 607, DateTimeKind.Local).AddTicks(8027), "test2" },
                    { 4, "tester3", "fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13", new DateTime(2020, 2, 1, 12, 37, 59, 607, DateTimeKind.Local).AddTicks(8029), "test3" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserInfo",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserInfo",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserInfo",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserInfo",
                keyColumn: "ID",
                keyValue: 4);
        }
    }
}
