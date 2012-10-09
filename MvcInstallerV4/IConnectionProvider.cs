using System;
using System.Linq;

namespace MvcInstaller
{
    public interface IConnectionProvider
    {
        string GetConnectionString();
    }
}
