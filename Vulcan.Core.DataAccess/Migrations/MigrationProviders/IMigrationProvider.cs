using System.Threading.Tasks;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders
{
    public interface IMigrationProvider
    {
        Task Process(DataDefinitionContext ddc);
    }
}
