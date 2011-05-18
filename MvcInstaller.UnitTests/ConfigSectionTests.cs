using MvcInstaller.Settings;
using NUnit.Framework;
using System;
using System.Configuration;

namespace MvcInstaller.UnitTests
{
    [TestFixture]
    public class ConfigSectionTests
    {

        [Test]
        public void Get_Config_Section()
        {
            System.Configuration.Configuration configSection = ConfigurationManager.OpenExeConfiguration(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MvcInstaller.UnitTests.dll.config"));
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
