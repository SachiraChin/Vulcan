using System;
using System.Threading.Tasks;
using Vulcan.Core.DataAccess.Models;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers
{
    public class FieldMigrationProvider : IMigrationProvider
    {
        public ExecutionType Type { get; set; }
        public Field Field { get; set; }
        public async Task Process(DataDefinitionContext ddc)
        {
            var field = Field;
            switch (Type)
            {
                case ExecutionType.Insert:
                case ExecutionType.InsertWithIdentity:
                    await ddc.AddFieldAsync(field,
                        Type == ExecutionType.InsertWithIdentity,
                        Type == ExecutionType.InsertWithIdentity);
                    break;
                case ExecutionType.Update:
                    await ddc.UpdateFieldAsync(field.Id, field);
                    break;
                case ExecutionType.Delete:
                    await ddc.DeleteFieldAsync(field.Id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
