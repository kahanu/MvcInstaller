using System;
using System.Data.SqlServerCe;
using System.Linq;
using MvcInstaller.Settings;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    public class SqlCe40ConnectionString : IConnectionProvider
    {
        #region ctors

        private readonly InstallerConfig config;

        public SqlCe40ConnectionString(InstallerConfig config)
        {
            this.config = config;
        }

        #endregion

        #region IConnectionProvider Members

        public string GetConnectionString()
        {
            return ConnectionStringBuilder(config.Database.InitialCatalog, config.Database.Password).ToString();
        }

        #endregion

        #region Private Methods

        private SqlCeConnectionStringBuilder ConnectionStringBuilder(string databaseName, string password)
        {
            SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder();

            string dbName = databaseName;
            if (databaseName.IndexOf(".sdf") == -1)
            {
                dbName = databaseName + ".sdf";
            }

            builder.PersistSecurityInfo = false;
            builder.DataSource = string.Format(@"|DataDirectory|\{0}", dbName);
            if (!string.IsNullOrEmpty(password))
            {
                builder.Password = password;
            }

            return builder;
        }

        #endregion
    }
}
