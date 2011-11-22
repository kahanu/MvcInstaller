using MvcInstaller.Settings;
using System.Web.Mvc.Html;

namespace MvcInstaller
{
    public class ConnectionStringComponent : IConnectionStringComponent
    {
        private InstallerConfig config;

        public ConnectionStringComponent(InstallerConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Dynamically create a connection string.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public string GetConnString()
        {
            return GetConnString(config, "");
        }

        public string GetConnString(InstallerConfig config, string dbName)
        {
            string connString = "Data Source=" + config.Database.DataSource.Trim() + ";";
            if (!string.IsNullOrEmpty(dbName))
            {
                connString += "Initial Catalog=" + dbName + ";";
            }
            else
            {
                connString += "Initial Catalog=" + config.Database.InitialCatalog + ";";
            }


            if (config.Database.UseTrustedConnection)
            {
                connString += "Integrated Security=true;";
            }
            else
            {
                connString += "User ID=" + config.Database.UserName.Trim() + ";Password=" + config.Database.Password + ";";
            }
            return connString;
        }

        public string BuildEntityFrameworkConnectionString()
        {
            string standardConn = GetConnString();
            string connString = string.Format("metadata=res://*/EntityFramework.{0}.csdl|res://*/EntityFramework.{0}.ssdl|res://*/EntityFramework.{0}.msl;provider=System.Data.SqlClient;provider connection string=&quot;{1}MultipleActiveResultSets=True&quot;", config.Database.EntityFrameworkEntitiesName, standardConn);

            return connString;
        }
    }
}
