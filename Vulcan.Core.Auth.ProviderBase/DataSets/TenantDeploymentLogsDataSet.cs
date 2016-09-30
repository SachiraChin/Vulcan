using System;
using System.Data;
using Dapper;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class TenantDeploymentLogsDataSet : DataSet
    {
        public TenantDeploymentLogsDataSet(DynamicDataContext context) : base(context)
        {
        }

        public void AddLogEntry(Guid tenantId, string logEntry)
        {
            Context.Connection.ExecuteScalar("[internal].[internal_TenantDeploymentLogs_Add]",
                new
                {
                    tenantId = tenantId,
                    logEntry = logEntry
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
