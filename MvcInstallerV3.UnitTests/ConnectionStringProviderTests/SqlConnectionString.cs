using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MvcInstaller.Settings;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    public class SqlConnectionString : ISqlConnection
    {
        #region ctors

        //private readonly string serverName;

        //private readonly string databaseName;

        //private readonly bool trustedConnection;

        //private readonly string userId;

        //private readonly string password;

        private readonly InstallerConfig config;

        public SqlConnectionString(InstallerConfig config)
        {
            this.config = config;
        }

        ///// <summary>
        ///// This constructor is for trusted connections.
        ///// </summary>
        ///// <param name="serverName"></param>
        ///// <param name="databaseName"></param>
        //public SqlConnectionString(string serverName, string databaseName):this(serverName, databaseName, true, null, null)
        //{
        //}

        ///// <summary>
        ///// This constructor uses credentials.
        ///// </summary>
        ///// <param name="serverName"></param>
        ///// <param name="databaseName"></param>
        ///// <param name="userId"></param>
        ///// <param name="password"></param>
        //public SqlConnectionString(string serverName, string databaseName, string userId, string password):this(serverName, databaseName, false, userId, password)
        //{
        //}

        ///// <summary>
        ///// This constructor is the base. If the trustedConnection parameter is true, the userId and password values will be ignored.
        ///// </summary>
        ///// <param name="serverName"></param>
        ///// <param name="databaseName"></param>
        ///// <param name="trustedConnection"></param>
        ///// <param name="userId"></param>
        ///// <param name="password"></param>
        //public SqlConnectionString(string serverName, string databaseName, bool trustedConnection, string userId, string password)
        //{
        //    this.databaseName = databaseName;
        //    this.serverName = serverName;
        //    this.password = password;
        //    this.userId = userId;
        //    this.trustedConnection = trustedConnection;
        //}

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
