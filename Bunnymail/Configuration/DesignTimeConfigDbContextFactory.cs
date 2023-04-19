using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bunnymail.Configuration
{
    public class DesignTimeConfigDbContextFactory : IDesignTimeDbContextFactory<ConfigDbContext>
    {
        public ConfigDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder().UseSqlite("Data Source=:memory:").Options;
            return new(options);
        }
    }
}
