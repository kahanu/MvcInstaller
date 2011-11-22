using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using MvcInstaller.Settings;
using System.Data.SqlClient;
using System.Web.Security;
using System.IO;

namespace MvcInstaller.SqlCeTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            Configuration configSection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Configuration configSection = WebConfigurationManager.OpenWebConfiguration(null);

            if (configSection == null)
            {
                throw new InvalidOperationException("Configuration file not available.");
            }
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            IConnectionStringComponent component = new ConnectionStringComponent(config);
            IConfigurationFactory factory = new ConfigurationFactory(component);
            factory.Execute(config, configSection);

            configSection.Save();
        }

        [TestMethod]
        public void get_usesqlce40_value_from_database_element_from_config_returns_true()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");

            Assert.AreEqual(true, config.Database.UseSqlCe40);
        }

        [TestMethod]
        public void when_usesqlce40_is_true_provider_name_is_sqlserverce_4_0()
        {
            // Arrange
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            IConnectionStringComponent component = new ConnectionStringComponent(config);

            // Act
            string actual = component.GetProviderName;

            // Assert
            Assert.AreEqual("System.Data.SqlServerCe.4.0", actual);
        }

        [TestMethod]
        public void when_usesqlce40_is_true_connection_string_is_for_sqlce()
        {
            // Arrange
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            IConnectionStringComponent component = new ConnectionStringComponent(config);

            // Act
            string actual = component.GetConnString();

            // Assert
            Assert.AreEqual(@"Data Source=|DataDirectory|\Database1.sdf;", actual);
        }

        [TestMethod]
        public void run_the_install_wizard()
        {
            // Arrange
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            config.Path.AppPath = @"C:\VSProjects\MvcInstaller\MvcInstaller.SqlCeTests\";

            // Act
            if (config.Membership.Create)
            {
                // Let's first verify that the RoleManager is enabled.
                if (!Roles.Enabled)
                    throw new ApplicationException("The RoleManager was not Enabled. It has been updated! Click &quot;Install&quot; to continue.");

                // Added: 5/19/2011 By King Wilder
                // Needed a way to validate rules based on the Membership section
                // in the web.config, such as minRequiredPasswordLength.  This
                // factory class will create rules based on these requirements and
                // validate the InstallerConfig values then display the error
                // messages back to the browser.
                //System.Configuration.Configuration configSection = WebConfigurationManager.OpenWebConfiguration("~");
                Configuration configSection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                IRulesValidationFactory rulesFactory = new RulesValidationFactory(config, configSection);
                if (!rulesFactory.Validate())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<p><b>There are some items that need your attention:</b></p>");
                    sb.Append("<ul>");
                    foreach (string error in rulesFactory.ValidationErrors)
                    {
                        sb.AppendFormat("<li>{0}</li>", error);
                    }
                    sb.Append("</ul>");
                    sb.Append("<p>Please fix these issues in the installer.config file and then come back and click the &quot;Install&quot; button.</p>");

                    throw new ApplicationException(sb.ToString());
                }

                // Add the ASPNETDB tables to the database using the SqlServices Install method.
                // This will add the ASPNETDB tables to the same database as the application.
                // NOTE: This method can ONLY be used for SQL Server.  To point to MySql, 
                // you will need to create the database scripts for MySql and add them to 
                // the RunScripts method, but then you need to have the MySql data provider
                // set in the web.config.
                //if (config.Database.UseTrustedConnection)
                //{
                //    // For SQL Server trusted connections
                //    try
                //    {
                //        System.Web.Management.SqlServices.Uninstall(config.Database.DataSource.Trim(), config.Database.InitialCatalog, System.Web.Management.SqlFeatures.All);
                //    }
                //    catch (SqlException)
                //    {
                //    }

                //    //DropASPNETDBTables(config);
                //    System.Web.Management.SqlServices.Install(config.Database.DataSource.Trim(), config.Database.InitialCatalog, System.Web.Management.SqlFeatures.All);
                //}
                //else
                //{
                //    // For SQL Server
                //    try
                //    {
                //        System.Web.Management.SqlServices.Uninstall(config.Database.DataSource.Trim(), config.Database.UserName, config.Database.Password, config.Database.InitialCatalog, System.Web.Management.SqlFeatures.All);
                //    }
                //    catch (SqlException)
                //    {
                //    }

                //    //DropASPNETDBTables(config);
                //    System.Web.Management.SqlServices.Install(config.Database.DataSource.Trim(), config.Database.UserName, config.Database.Password, config.Database.InitialCatalog, System.Web.Management.SqlFeatures.All);
                //}

                InstallASPNETDBTables(config);
            }


        }

        private void InstallASPNETDBTables(InstallerConfig config)
        {
            string[] files = Directory.GetFiles(config.Path.AppPath + @"App_Data\scripts");
            foreach (string file in files)
            {
                string[] statements = GetScriptStatements(File.ReadAllText(file, new System.Text.UTF8Encoding()), StringSplitOptions.None);
                ExecuteStatements(statements, config);
            }
        }

        private string[] GetScriptStatements(string p)
        {
            return GetScriptStatements(p, StringSplitOptions.RemoveEmptyEntries);
        }

        private string[] GetScriptStatements(string p, StringSplitOptions options)
        {
            string[] statements = p.Split(new string[] { "GO\r\n" }, options);
            return statements;
        }

        private void ExecuteStatements(string[] tableStatements, InstallerConfig config)
        {
            if (tableStatements.Length > 0)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    IConnectionStringComponent component = new ConnectionStringComponent(config);
                    conn.ConnectionString = component.GetConnString();
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(string.Empty, conn))
                    {
                        foreach (string statement in tableStatements)
                        {
                            command.CommandText = statement;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
