using System.Data;
using System.Threading.Tasks;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers
{
    public class SchemaMigrationProvider : IMigrationProvider
    {
        public string SchemaName { get; set; }
        public async Task Process(DataDefinitionContext ddc)
        {
            await ddc.ExecuteScalarAsync<int>(
                "core",
                "[base_Migrations_MigrateSchema]",
                new
                {
                    SchemaName = SchemaName
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
