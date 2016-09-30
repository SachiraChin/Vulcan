using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class TenantDataSet : DataSet
    {
        public TenantDataSet(DynamicDataContext context) : base(context)
        {
        }

        public async Task SetStoreCreated(Guid tenantId)
        {
            await Context.Connection.ExecuteScalarAsync("[internal].[internal_InternalTenant_SetStoreCreated]",
                new
                {
                    tenantId = tenantId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task SetInitialConfigCompleted(Guid tenantId)
        {
            await Context.Connection.ExecuteScalarAsync("[internal].[internal_InternalTenant_SetInitialConfigCompleted]",
                new
                {
                    tenantId = tenantId
                },
                commandType: CommandType.StoredProcedure);
        }

        public void SetTempData(Guid tenantId, string tempData)
        {
            Context.Connection.ExecuteScalar("[internal].[internal_InternalTenant_SetTempData]",
                new
                {
                    tenantId = tenantId,
                    tempData = tempData
                },
                commandType: CommandType.StoredProcedure);
        }

        public void SetStoreCreateKey(Guid tenantId, string hash, string salt)
        {
            Context.Connection.ExecuteScalar("[internal].[internal_InternalTenant_SetStoreCreateJobKey]",
                new
                {
                    tenantId = tenantId,
                    hash = hash,
                    salt = salt
                },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<Tenant> GetTenant(Guid tenantId)
        {
            return (await Context.Connection.QueryAsync<Tenant>("[internal].[internal_InternalTenant_GetById]",
                new
                {
                    tenantId = tenantId
                },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }
    }
}
