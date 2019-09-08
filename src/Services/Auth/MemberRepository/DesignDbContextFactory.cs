using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace MemberRepository
{
    internal class DesignDbContextFactory : IDesignTimeDbContextFactory<MemberContext>
    {
        public MemberContext CreateDbContext(params string[] args)
        {
            if (args.Length == 0)
                throw new Exception("請提供連線字串");

            string connectionString = args[0];

            DbContextOptionsBuilder<MemberContext> builder = new DbContextOptionsBuilder<MemberContext>();
            builder.UseSqlServer(connectionString);
            return new MemberContext(builder.Options);
        }
    }
}
