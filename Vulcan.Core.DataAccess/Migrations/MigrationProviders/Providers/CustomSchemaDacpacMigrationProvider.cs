using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers
{
    public class CustomSchemaDacpacMigrationProvider : IMigrationProvider
    {
        public Action<string> LogWriter { get; set; }
        public string OriginalDacpacPath { get; set; }
        public string DestinationDacpacPath { get; set; }
        public Dictionary<string, string> UpdatedSchemaList { get; set; }
        public Dictionary<string, string> SqlCmdParameterList { get; set; }
        public Task Process(DataDefinitionContext ddc)
        {
            var dacPacPath = OriginalDacpacPath;
            if (DestinationDacpacPath != null)
            {
                dacPacPath = DestinationDacpacPath;
            }
            var fileExists = false;
            if (UpdatedSchemaList.Count > 0)
                fileExists = !DacFxUtils.SubstituteSchemaInDacpac(OriginalDacpacPath, UpdatedSchemaList, DestinationDacpacPath, logWriter: LogWriter);

            if (!fileExists)
            {
                DacFxUtils.DeployDacpac(dacPacPath, ddc.Connection.ConnectionString, ddc.Connection.Database, SqlCmdParameterList, LogWriter);
            }

            return Task.FromResult<object>(null);
        }
    }
}
