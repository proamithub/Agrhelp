
using HelpData.Dapper.Interfaces;

namespace HelpData.Dapper
{
    public interface IDapperContext
    {
        IQueryHelper QueryHelper { get; }
        IProcedureHelper ProcedureHelper { get; }
    }
}
