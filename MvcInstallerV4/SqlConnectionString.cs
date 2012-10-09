using System;
using System.Data.SqlClient;
using System.Linq;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    public class SqlConnectionString : ISqlConnection
    {
        #region ctors

        private readonly InstallerConfig config;

        public SqlConnectionString(InstallerConfig config)
        {
            this.config = config;
        }

        #endregion

        #region IConnectionProvider Members

        public string GetConnectionString()
        {
            return BuildStandardConnectionString(config.Database.DataSource, config.Database.InitialCatalog, config.Database.UseTrustedConnection, config.Database.UserName, config.Database.Password).ToString();
        }

        #endregion

        #region Private Methods

        private SqlConnectionStringBuilder BuildStandardConnectionString(string serverName, string databaseName, bool trustedConnection, string userId, string password)
        {
            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;

            if (trustedConnection)
            {
                sqlBuilder.IntegratedSecurity = true;
            }
            else
            {
                sqlBuilder.UserID = userId;
                sqlBuilder.Password = password;
            }

            return sqlBuilder;
        }

        #endregion

        #region ISqlConnection Members

        public SqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            return BuildStandardConnectionString(config.Database.DataSource, config.Database.InitialCatalog, config.Database.UseTrustedConnection, config.Database.UserName, config.Database.Password);
        }

        #endregion
    }
}
