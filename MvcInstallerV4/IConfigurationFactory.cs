using MvcInstaller.Settings;
using System.Configuration;

namespace MvcInstaller
{
    public interface IConfigurationFactory
    {
        void Execute(InstallerConfig config, Configuration configSection);
    }
}
