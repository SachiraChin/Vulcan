using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Vulcan.Core.Auth.DataSets;
using Vulcan.Core.DataAccess;
using Vulcan.Core.DataAccess.Migrations;
using Vulcan.Core.DataAccess.Migrations.MigrationProviders;
using Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers;

namespace Vulcan.Core.Auth.DataContexts
{
    public class InternalDataContext : DynamicDataContext
    {
        public InternalDataContext() : base("InternalDataContext", null)
        {
        }

        private TenantDataSet _tenants;
        public TenantDataSet Tenants
        {
            get { return _tenants ?? (_tenants = new TenantDataSet(this)); }
            set { _tenants = value; }
        }

        private ExternalTenantDataSet _externalTenants;
        private TenantDeploymentLogsDataSet _tenantDeploymentLogsDataSet;

        public ExternalTenantDataSet ExternalTenants
        {
            get { return _externalTenants ?? (_externalTenants = new ExternalTenantDataSet(this)); }
            set { _externalTenants = value; }
        }

        public TenantDeploymentLogsDataSet TenantDeploymentLogsDataSet
        {
            get { return _tenantDeploymentLogsDataSet ?? (_tenantDeploymentLogsDataSet = new TenantDeploymentLogsDataSet(this)); }
            set { _tenantDeploymentLogsDataSet = value; }
        }

        public async Task CreateTenantDatastore(string tenantId, Action<string> logWriter)
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

#if DEBUG
            path += @"\Dacpacs\Debug";
#else
            path += @"\Dacpacs\Release";
#endif
            var migration = new Migration
            {
                MigrationId = Guid.NewGuid(),
                MigrationEntries = new List<IMigrationProvider>()
                    {
                        new CustomSchemaDacpacMigrationProvider()
                        {
                            OriginalDacpacPath = path + @"\Vulcan.Core.Database.dacpac",
                            DestinationDacpacPath = path + $@"\Generated\{tenantId}_Vulcan.Core.Database.dacpac",
                            UpdatedSchemaList =  new Dictionary<string, string>()
                                {
                                    ["core"] = $"{tenantId}_core"
                                },
                            SqlCmdParameterList = new Dictionary<string, string>()
                                {
                                    ["NewSchema"] = $"{tenantId}_core"
                                },
                            LogWriter = logWriter
                        },
                        new CustomSchemaDacpacMigrationProvider()
                        {
                            OriginalDacpacPath = path + @"\Vulcan.Core.Auth.Database.dacpac",
                            DestinationDacpacPath = path + $@"\Generated\{tenantId}_Vulcan.Core.Auth.Database.dacpac",
                            UpdatedSchemaList =  new Dictionary<string, string>()
                                {
                                    ["auth"] = $"{tenantId}_auth"
                                },
                            SqlCmdParameterList = new Dictionary<string, string>()
                                {
                                    ["NewSchema"] = $"{tenantId}_auth"
                                },
                            LogWriter = logWriter
                        },
                        new CustomSchemaDacpacMigrationProvider()
                        {
                            OriginalDacpacPath = path + @"\Vulcan.Core.Auth.AzureAuthProvider.Database.dacpac",
                            DestinationDacpacPath = path + $@"\Generated\{tenantId}_Vulcan.Core.Auth.AzureAuthProvider.Database.dacpac",
                            UpdatedSchemaList =  new Dictionary<string, string>()
                                {
                                    ["aad"] = $"{tenantId}_aad"
                                },
                            SqlCmdParameterList = new Dictionary<string, string>()
                                {
                                    ["NewSchema"] = $"{tenantId}_aad"
                                },
                            LogWriter = logWriter
                        }
                    }
            };


            var migrationMan = new MigrationManager(new DataDefinitionContext("SharedDataContext", null));
            await migrationMan.Migrate(migration, true);
        }
    }
}
