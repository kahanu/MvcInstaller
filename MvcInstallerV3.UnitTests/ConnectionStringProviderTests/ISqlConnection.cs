using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace MvcInstallerV3.UnitTests.ConnectionStringProviderTests
{
    public interface ISqlConnection : IConnectionProvider
    {
        SqlConnectionStringBuilder GetConnectionStringBuilder();
    }
}
