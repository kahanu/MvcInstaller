using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcInstaller;
using System.Data.SqlClient;
using System.Data.EntityClient;

namespace MvcInstallerV3.UnitTests.IConnectionStringComponentTests
{
    public class FakeConnectionStringComponent : IConnectionStringComponent
    {
        #region IConnectionStringComponent Members

        public string GetConnString()
        {
            throw new NotImplementedException();
        }

        public string BuildEntityFrameworkConnectionString()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IProviderName Members

        public string GetProviderName
        {
            get { throw new NotImplementedException(); }
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
    }
}
