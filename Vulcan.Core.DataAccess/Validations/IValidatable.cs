using System.Web.Http.ModelBinding;
using Vulcan.Core.DataAccess.Entities;

namespace Vulcan.Core.DataAccess.Validations
{
    public interface IValidatable : IEntity
    {
        //List<IValidator> Validators { get; set; }
        //List<ValidateMessage> ValidateMessages { get; set; }
        ModelStateDictionary ModelState { get; set; }
    }
}
