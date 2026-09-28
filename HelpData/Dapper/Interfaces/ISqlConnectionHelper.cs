
using System.Data;

namespace HelpData.Dapper.Interfaces
{
    public interface ISqlConnectionHelper
    {
        IDbConnection GetDbConnection();
    }
}
