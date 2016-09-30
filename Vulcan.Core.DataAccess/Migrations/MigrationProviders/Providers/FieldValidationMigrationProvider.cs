using System;
using System.Threading.Tasks;
using Vulcan.Core.DataAccess.Models;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers
{
    public class FieldValidationMigrationProvider : IMigrationProvider
    {
        public ExecutionType Type { get; set; }
        public FieldValidation FieldValidation { get; set; }
        public int FieldId { get; set; }
        public async Task Process(DataDefinitionContext ddc)
        {
            switch (Type)
            {
                case ExecutionType.Insert:
                case ExecutionType.InsertWithIdentity:
                    if (FieldId == 0)
                    {
                        throw new Exception("Field id not provided");
                    }
                    await ddc.AddValidationAsync(FieldId, FieldValidation);
                    break;
                case ExecutionType.Update:
                    await ddc.UpdateValidationAsync(FieldId, FieldValidation.Id, FieldValidation);
                    break;
                case ExecutionType.Delete:
                    await ddc.DeleteValidationAsync(FieldValidation.Id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
