
using MemberRepository.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberRepository
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
        }
    }
}
