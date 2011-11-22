using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcInstaller.Settings;
using MvcInstaller;
using System.Web.Configuration;
using System.Xml.Linq;
using System.IO;

namespace MvcInstallerV3.UnitTests
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
            //System.Configuration.Configuration configSection = ConfigurationManager.OpenExeConfiguration(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MvcInstallerV3.UnitTests.dll.config"));
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

            Console.WriteLine("Before save...");
            var result = configSection.ConnectionStrings.ConnectionStrings[0].ConnectionString;
            Console.WriteLine(result);

            configSection.Save();

            Console.WriteLine("After save...");
            result = configSection.ConnectionStrings.ConnectionStrings[0].ConnectionString;
            Console.WriteLine(result);

            string connString = configSection.ConnectionStrings.ConnectionStrings[0].ConnectionString;
            Console.WriteLine(connString);

        }

        [TestMethod]
        public void modify_connection_string()
        {
            Fix2();
        }

        private void Fix()
        {
            XElement xml = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"\MvcInstallerV3.UnitTests.DLL.config");

            if (xml != null)
            {
                XElement connStringElement = xml.Element("connectionStrings");
                XElement addElement = connStringElement.Elements("add").First();
                string connString = addElement.Attribute("connectionString").Value;
                connString = connString.Replace("amp;", "");
                addElement.Attribute("connectionString").Value = connString;
                xml.Save(AppDomain.CurrentDomain.BaseDirectory + @"\MvcInstallerV3.UnitTests.DLL.config");
            }
        }

        private void Fix2()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\MvcInstallerV3.UnitTests.DLL.config"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace("amp;", "");
                    sb.AppendLine(line);
                }
            }

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\MvcInstallerV3.UnitTests.DLL.config"))
            {
                sw.Write(sb.ToString());
            }
        }
    }
}
