using MvcInstaller.Settings;

namespace MvcInstaller
{
    public interface IConnectionStringComponent
    {
        string GetConnString();
        string BuildEntityFrameworkConnectionString();
    }
}
