using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MyServer.Data
{
    public class TemporaryDbContextFactory : IDbContextFactory<MyServerDbContext>
    {
        public MyServerDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<MyServerDbContext>();
            builder.UseSqlServer("Server=.;Database=MyServer;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new MyServerDbContext(builder.Options);
        }
    }
}