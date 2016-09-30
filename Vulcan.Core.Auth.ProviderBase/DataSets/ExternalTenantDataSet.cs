using System.Data;
using Dapper;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class ExternalTenantDataSet : DataSet
    {
        public ExternalTenantDataSet(DynamicDataContext context) : base(context)
        {
        }

        public string GetInternalByExternalTenantId(string externalTenantId)
        {
            return Context.Connection.ExecuteScalar<string>("[internal].[internal_InternalTenant_GetByExternalTenantId]", 
                new { ExternalTenantId  = externalTenantId},
                commandType: CommandType.StoredProcedure);
        }
        
        public string AddExternalTenant(string externalTenantId)
        {
            return Context.Connection.ExecuteScalar<string>("[internal].[internal_ExternalTenants_Add]",
                new { ExternalTenantId = externalTenantId },
                commandType: CommandType.StoredProcedure);
        }
    }
}
