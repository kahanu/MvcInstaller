using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using MvcInstaller.Settings;
using MvcInstaller;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    [TestClass]
    public class ConnectionStringProviderTests
    {
        
        #region Standard Sql Connection Tests

        [TestMethod]
        public void standard_sql_connection_string_with_integrated_security()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = true;

            ISqlConnection provider = new SqlConnectionString(config);

            // Act
            var actual = provider.GetConnectionString();
            var expected = @"Data Source=kingwilder-pc\sqlserver;Initial Catalog=BloggerApp;Integrated Security=True";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void standard_sql_connection_string_with_userid_password()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "CommerceApp";
            config.Database.UseTrustedConnection = false;
            config.Database.UserName = "dbadminuser";
            config.Database.Password = "dbadminpassword";

            IConnectionProvider provider = new SqlConnectionString(config);

            // Act
            var actual = provider.GetConnectionString();
            var expected = @"Data Source=kingwilder-pc\sqlserver;Initial Catalog=CommerceApp;User ID=dbadminuser;Password=dbadminpassword";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void standard_sql_connection_string_with_all_parameters_returns_integrated_security()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "CommerceApp";
            config.Database.UseTrustedConnection = true;
            config.Database.UserName = "dbadminuser";
            config.Database.Password = "dbadminpassword";

            IConnectionProvider provider = new SqlConnectionString(config);

            // Act
            var actual = provider.GetConnectionString();
            var expected = @"Data Source=kingwilder-pc\sqlserver;Initial Catalog=CommerceApp;Integrated Security=True";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void standard_sql_connection_string_with_all_parameters_returns_credentials()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "CommerceApp";
            config.Database.UseTrustedConnection = false;
            config.Database.UserName = "samAdmin";
            config.Database.Password = "myPassword";

            IConnectionProvider provider = new SqlConnectionString(config);

            // Act
            var actual = provider.GetConnectionString();
            var expected = @"Data Source=kingwilder-pc\sqlserver;Initial Catalog=CommerceApp;User ID=samAdmin;Password=myPassword";

            // Assert
            Assert.AreEqual(expected, actual);
        } 
        #endregion

        #region EntityFramework Connection Tests

        [TestMethod]
        public void entityframework_connection_composes_sqlconnectionstring_using_trusted_connection()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = true;
            config.Database.EntityFrameworkEntitiesName = "BloggerEntities";

            IConnectionProvider efprovider = new EntityFrameworkConnectionString(config);

            // Act
            var actual = efprovider.GetConnectionString();
            var expected = @"metadata=res://*/Models.BloggerEntities.csdl|res://*/Models.BloggerEntities.ssdl|res://*/Models.BloggerEntities.msl;provider=System.Data.SqlClient;";
            expected += @"provider connection string=""Data Source=kingwilder-pc\sqlserver;Initial Catalog=BloggerApp;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework""";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void entityframework_connection_composes_sqlconnectionstring_using_credentials()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = false;
            config.Database.EntityFrameworkEntitiesName = "BloggerEntities";
            config.Database.UserName = "dbadminuser";
            config.Database.Password = "dbadminpassword";

            IConnectionProvider efprovider = new EntityFrameworkConnectionString(config);

            // Act
            var actual = efprovider.GetConnectionString();
            var expected = @"metadata=res://*/Models.BloggerEntities.csdl|res://*/Models.BloggerEntities.ssdl|res://*/Models.BloggerEntities.msl;provider=System.Data.SqlClient;";
            expected += @"provider connection string=""Data Source=kingwilder-pc\sqlserver;Initial Catalog=BloggerApp;User ID=dbadminuser;Password=dbadminpassword;MultipleActiveResultSets=True;Application Name=EntityFramework""";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region SqlCe Connection Tests

        [TestMethod]
        public void build_sqlce_connection_with_filename_extension()
        {
            // Arrange
            SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder();
            string databaseName = "BlogApp.sdf";
            string dbName = databaseName;
            if (databaseName.IndexOf(".sdf") == -1)
            {
                dbName = databaseName + ".sdf";
            }

            builder.DataSource = string.Format(@"|DataDirectory|\{0}", dbName);

            // Act
            var actual = builder.ToString();
            var expected = @"Data Source=|DataDirectory|\BlogApp.sdf";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void build_sqlce_connection_without_filename_extension()
        {
            // Arrange
            SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder();
            string databaseName = "BlogApp";
            string dbName = databaseName;
            if (databaseName.IndexOf(".sdf") == -1)
            {
                dbName = databaseName + ".sdf";
            }

            builder.DataSource = string.Format(@"|DataDirectory|\{0}", dbName);

            // Act
            var actual = builder.ToString();
            var expected = @"Data Source=|DataDirectory|\BlogApp.sdf";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void build_sqlce_connection_with_password()
        {
            // Arrange
            SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder();
            string databaseName = "BlogApp";
            string password = "myPassword";

            string dbName = databaseName;
            if (databaseName.IndexOf(".sdf") == -1)
            {
                dbName = databaseName + ".sdf";
            }

            builder.DataSource = string.Format(@"|DataDirectory|\{0}", dbName);
            if (!string.IsNullOrEmpty(password))
            {
                builder.Password = password;
            }

            // Act
            var actual = builder.ToString();
            var expected = @"Data Source=|DataDirectory|\BlogApp.sdf;Password=myPassword";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region SqlCe Connection Builder Tests

        //[TestMethod]
        //public void sqlce_builder_without_file_extension_or_password()
        //{
        //    // Arrange
        //    InstallerConfig config = new InstallerConfig();
        //    config.Database.InitialCatalog = "BloggerApp";

        //    IConnectionProvider provider = new SqlCe40ConnectionString(config);

        //    // Act
        //    var actual = provider.GetConnectionString();
        //    var expected = @"Data Source=|DataDirectory|\BloggerApp.sdf;Persist Security Info=False";

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void sqlce_builder_with_file_extension_no_password()
        //{
        //    // Arrange
        //    InstallerConfig config = new InstallerConfig();
        //    config.Database.InitialCatalog = "BlogManApp.sdf";

        //    IConnectionProvider provider = new SqlCe40ConnectionString(config);

        //    // Act
        //    var actual = provider.GetConnectionString();
        //    var expected = @"Data Source=|DataDirectory|\BlogManApp.sdf;Persist Security Info=False";

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void sqlce_builder_with_file_extension_and_password()
        //{
        //    // Arrange
        //    InstallerConfig config = new InstallerConfig();
        //    config.Database.InitialCatalog = "BlogManApp.sdf";
        //    config.Database.Password = "dbadminpassword";

        //    IConnectionProvider provider = new SqlCe40ConnectionString(config);

        //    // Act
        //    var actual = provider.GetConnectionString();
        //    var expected = @"Data Source=|DataDirectory|\BlogManApp.sdf;Password=dbadminpassword;Persist Security Info=False";

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

        #endregion
    }
}
