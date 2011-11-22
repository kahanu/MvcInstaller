using System;
using System.Linq;
using MvcInstaller.Settings;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    public class ConnectionFactory
    {
        private IConnectionProvider provider = null;

        private readonly InstallerConfig _config;

        public ConnectionFactory(InstallerConfig config)
        {
            this._config = config;
        }

        public string ConnectionString()
        {
            if (!string.IsNullOrEmpty(_config.Database.EntityFrameworkEntitiesName))
            {
                provider = new EntityFrameworkConnectionString(_config);
            }
            else if (_config.Database.UseSqlCe40)
            {
                provider = new SqlCe40ConnectionString(_config);
            }
            else
            {
                provider = new SqlConnectionString(_config);
            }

            return provider.GetConnectionString();
        }
    }
}
