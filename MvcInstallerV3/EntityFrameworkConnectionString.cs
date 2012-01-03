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

        private readonly InstallerConfig config;

        public EntityFrameworkConnectionString(InstallerConfig config)
        {
            this.config = config;
        }

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
            entityBuilder.Metadata = string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", entityName);

            return entityBuilder;
        }

        #endregion
    }
}
