using Microsoft.EntityFrameworkCore;

namespace Bunnymail.Configuration
{
    public class ConfigDbContextFactory : IDbContextFactory<ConfigDbContext>
    {
        private static ConfigDbContextFactory? _instance;

        public static ConfigDbContextFactory Instance => _instance ?? throw new NotImplementedException("ConfigDbContextFactory was called before being implemented.");

        private DbContextOptions options;

        public static ConfigDbContextFactory Initialise(IConfiguration config)
        {
            if (_instance != null)
                throw new Exception("Double initialisation of ConfigDbContextFactory.  Ensure this class is only initialised once.");
            _instance = new(config);
            return Instance;
        }

        private ConfigDbContextFactory(IConfiguration config)
        {
            string connectionString = config["database"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Expected 'database' config parameter to contain Sqlite DB connection string.  It does not.");
            }

            DbContextOptionsBuilder optionsBuilder = new();
            optionsBuilder.UseSqlite(connectionString);

            options = optionsBuilder.Options;
        }

        public ConfigDbContext CreateDbContext() => new(options);

    }
}
