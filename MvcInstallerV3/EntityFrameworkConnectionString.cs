using System;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    public class EntityFrameworkConnectionString : IConnectionProvider
    {

        #region ctors

        //private readonly string entityName;

        //private readonly string serverName;

        //private readonly string databaseName;

        //private readonly string userId;

        //private readonly string password;

        //private readonly bool trustedConnection;

        private readonly InstallerConfig config;

        public EntityFrameworkConnectionString(InstallerConfig config)
        {
            this.config = config;
        }

        //public EntityFrameworkConnectionString(string serverName, string databaseName, string entityName):this(serverName, databaseName, true, null, null, entityName)
        //{
        //}

        //public EntityFrameworkConnectionString(string serverName, string databaseName, string userId, string password, string entityName)
        //    :this(serverName, databaseName, false, userId, password, entityName)
        //{
        //}

        //public EntityFrameworkConnectionString(string serverName, string databaseName, bool trustedConnection, string userId, string password, string entityName)
        //{
        //    this.trustedConnection = trustedConnection;
        //    this.databaseName = databaseName;
        //    this.serverName = serverName;
        //    this.password = password;
        //    this.userId = userId;
        //    this.entityName = entityName;
        //}

        #endregion

        #region IConnectionProvider Members

        public string GetConnectionString()
        {
            ISqlConnection sqlprovider = new SqlConnectionString(config);
            SqlConnectionStringBuilder sqlBuilder = sqlprovider.GetConnectionStringBuilder();

            return BuildEntityFrameworkConnection(sqlBuilder, config.Database.EntityFrameworkEntitiesName).ToString();
        }

        #endregion

        #region Private Methods

        private EntityConnectionStringBuilder BuildEntityFrameworkConnection(SqlConnectionStringBuilder sqlBuilder, string entityName)
        {
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = sqlBuilder.ToString();
            entityBuilder.Metadata = string.Format(@"res://*/Models.{0}.csdl|res://*/Models.{0}.ssdl|res://*/Models.{0}.msl", entityName);

            return entityBuilder;
        }

        #endregion
    }
}
