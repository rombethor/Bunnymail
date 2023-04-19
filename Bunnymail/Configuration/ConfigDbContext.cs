using Bunnymail.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bunnymail.Configuration
{
    public class ConfigDbContext : DbContext
    {
        public ConfigDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<EventTemplate> EventTemplates => Set<EventTemplate>();

    }
}
