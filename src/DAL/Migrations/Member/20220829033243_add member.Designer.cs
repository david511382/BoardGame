﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations.Member
{
    [DbContext(typeof(MemberContext))]
    [Migration("20220829033243_add member")]
    partial class addmember
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DAL.Models.UserInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(10);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<DateTime>("RegisterTime");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.HasKey("ID");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.HasIndex("Username", "Password");

                    b.ToTable("UserInfo");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "tester",
                            Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                            RegisterTime = new DateTime(2022, 8, 29, 11, 32, 43, 700, DateTimeKind.Local).AddTicks(8303),
                            Username = "test"
                        },
                        new
                        {
                            ID = 2,
                            Name = "tester1",
                            Password = "1b4f0e9851971998e732078544c96b36c3d01cedf7caa332359d6f1d83567014",
                            RegisterTime = new DateTime(2022, 8, 29, 11, 32, 43, 701, DateTimeKind.Local).AddTicks(3056),
                            Username = "test1"
                        },
                        new
                        {
                            ID = 3,
                            Name = "tester2",
                            Password = "60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752",
                            RegisterTime = new DateTime(2022, 8, 29, 11, 32, 43, 701, DateTimeKind.Local).AddTicks(3057),
                            Username = "test2"
                        },
                        new
                        {
                            ID = 4,
                            Name = "tester3",
                            Password = "fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13",
                            RegisterTime = new DateTime(2022, 8, 29, 11, 32, 43, 701, DateTimeKind.Local).AddTicks(3058),
                            Username = "test3"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
