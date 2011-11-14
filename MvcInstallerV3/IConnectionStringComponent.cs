using MvcInstaller.Settings;

namespace MvcInstaller
{
    public interface IConnectionStringComponent : IProviderName
    {
        string GetConnString();
        string BuildEntityFrameworkConnectionString();
    }
}
