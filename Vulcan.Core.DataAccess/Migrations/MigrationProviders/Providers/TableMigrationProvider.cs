using System.Data;
using System.Threading.Tasks;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers
{
    public class TableMigrationProvider : IMigrationProvider
    {
        public string TableName { get; set; }
        public async Task Process(DataDefinitionContext ddc)
        {
            await ddc.ExecuteScalarAsync<int>(
                "core",
                "[base_Migration_MigrateTable]",
                new
                {
                    TableName = TableName
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
