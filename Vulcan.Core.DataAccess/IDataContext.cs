using System.Data;

namespace Vulcan.Core.DataAccess
{
    public interface IDataContext
    {
        IDbConnection Connection { get; }
        string TenantId { get; }
    }
}
