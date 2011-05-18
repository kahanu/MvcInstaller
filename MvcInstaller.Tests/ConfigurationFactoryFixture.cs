using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcInstaller.Settings;
using System;
using System.Web.Configuration;

namespace MvcInstaller.Tests
{
    /// <summary>
    /// Summary description for ConfigurationFactoryFixture
    /// </summary>
    [TestClass]
    public class ConfigurationFactoryFixture
    {
        public ConfigurationFactoryFixture()
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
        public void Update_configuration_and_save()
        {
            //string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MvcInstaller.Tests.dll.config");
            string path = @"C:\VSProjects\MvcInstaller\MvcInstaller.Tests\web.config";

            //System.Configuration.Configuration configSection = ConfigurationManager.OpenExeConfiguration(path);
            System.Configuration.Configuration configSection = WebConfigurationManager.OpenWebConfiguration(path);
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
    }
}
