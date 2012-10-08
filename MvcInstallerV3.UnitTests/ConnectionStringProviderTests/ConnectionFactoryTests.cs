using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcInstaller.Settings;
using MvcInstaller;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    [TestClass]
    public class ConnectionFactoryTests
    {
        [TestMethod]
        public void return_standard_connection_string_using_trustedconnection()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = true;
            
            ConnectionFactory factory = new ConnectionFactory(config);

            // Act
            var actual = factory.ConnectionString();
            var expected = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", config.Database.DataSource, config.Database.InitialCatalog);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void return_standard_connection_string_with_credentials()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = false;
            config.Database.UserName = "dbadminuser";
            config.Database.Password = "dbadminpassword";

            ConnectionFactory factory = new ConnectionFactory(config);

            // Act
            var actual = factory.ConnectionString();
            var expected = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", config.Database.DataSource, config.Database.InitialCatalog, config.Database.UserName, config.Database.Password);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void return_entityframework_connection_using_trustedconnection()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = true;
            config.Database.EntityFrameworkEntitiesName = "EntityFramework.BloggerEntities";

            ConnectionFactory factory = new ConnectionFactory(config);

            // Act
            var actual = factory.ConnectionString();
            //var expected = @"metadata=res://*/Models.BloggerEntities.csdl|res://*/Models.BloggerEntities.ssdl|res://*/Models.BloggerEntities.msl;provider=System.Data.SqlClient;provider connection string=""Data Source=kingwilder-pc\sqlserver;Initial Catalog=BloggerApp;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework""";
            var expected = @"metadata=res://*/EntityFramework.BloggerEntities.csdl|res://*/EntityFramework.BloggerEntities.ssdl|res://*/EntityFramework.BloggerEntities.msl;provider=System.Data.SqlClient;provider connection string=""Data Source=kingwilder-pc\sqlserver;Initial Catalog=BloggerApp;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework""";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void return_entityframework_connection_using_credentials()
        {
            // Arrange
            InstallerConfig config = new InstallerConfig();
            config.Database.DataSource = @"kingwilder-pc\sqlserver";
            config.Database.InitialCatalog = "BloggerApp";
            config.Database.UseTrustedConnection = false;
            config.Database.EntityFrameworkEntitiesName = "EntityFramework.BloggerEntities";
            config.Database.UserName = "dbadminuser";
            config.Database.Password = "dbadminpassword";

            ConnectionFactory factory = new ConnectionFactory(config);

            // Act
            var actual = factory.ConnectionString();
            var expected = @"metadata=res://*/EntityFramework.BloggerEntities.csdl|res://*/EntityFramework.BloggerEntities.ssdl|res://*/EntityFramework.BloggerEntities.msl;provider=System.Data.SqlClient;provider connection string=""Data Source=kingwilder-pc\sqlserver;Initial Catalog=BloggerApp;User ID=dbadminuser;Password=dbadminpassword;MultipleActiveResultSets=True;Application Name=EntityFramework""";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod]
        //public void return_sqlce40_connection_with_no_password()
        //{
        //    // Arrange
        //    InstallerConfig config = new InstallerConfig();
        //    config.Database.InitialCatalog = "BloggerApp";
        //    config.Database.UseSqlCe40 = true;

        //    ConnectionFactory factory = new ConnectionFactory(config);

        //    // Act
        //    var actual = factory.ConnectionString();
        //    var expected = @"Data Source=|DataDirectory|\BloggerApp.sdf;Persist Security Info=False";

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}
    }
}
