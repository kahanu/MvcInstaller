using System;
using System.Data.SqlClient;
using System.Linq;

namespace MvcInstaller
{
    public interface ISqlConnection : IConnectionProvider
    {
        SqlConnectionStringBuilder GetConnectionStringBuilder();
    }
}
