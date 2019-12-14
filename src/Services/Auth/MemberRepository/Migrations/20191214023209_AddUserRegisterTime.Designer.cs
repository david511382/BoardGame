﻿// <auto-generated />
using System;
using MemberRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MemberRepository.Migrations
{
    [DbContext(typeof(MemberContext))]
    [Migration("20191214023209_AddUserRegisterTime")]
    partial class AddUserRegisterTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MemberRepository.Models.UserInfo", b =>
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
                });
#pragma warning restore 612, 618
        }
    }
}
