
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL
{
    public class MemberContext : DbContext
    {
        public DbSet<UserInfo> UserInfos { get; set; }

        public MemberContext(DbContextOptions<MemberContext> options)
              : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>()
              .HasIndex(i => new { i.Username, i.Password });
            modelBuilder.Entity<UserInfo>()
                .HasIndex(c => c.Username)
                .IsUnique();


            modelBuilder.Entity<UserInfo>().HasData(
                new UserInfo()
                {
                    ID = 1,
                    Name = "tester",
                    Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                    Username = "test",
                    RegisterTime = DateTime.Now
                },
                new UserInfo()
                {
                    ID = 2,
                    Name = "tester1",
                    Password = "1b4f0e9851971998e732078544c96b36c3d01cedf7caa332359d6f1d83567014",
                    Username = "test1",
                    RegisterTime = DateTime.Now
                },
                new UserInfo()
                {
                    ID = 3,
                    Name = "tester2",
                    Password = "60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752",
                    Username = "test2",
                    RegisterTime = DateTime.Now
                },
                new UserInfo()
                {
                    ID = 4,
                    Name = "tester3",
                    Password = "fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13",
                    Username = "test3",
                    RegisterTime = DateTime.Now
                }
             );
        }
    }
}
