using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    public interface IConnectionProvider
    {
        string GetConnectionString();
    }
}
