using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace Vulcan.Core.DataAccess.Validations
{
    public class ValidationProvider
    {
        public bool Validate(IValidatable obj, List<IValidator> validators)
        {
            var valid = true;

            obj.ModelState = new ModelStateDictionary();
            foreach (var validator in validators)
            {
                var val = obj.EntityData.ContainsKey(validator.FieldName.ToLower()) ? obj.EntityData[validator.FieldName.ToLower()] : (obj.EntityData.ContainsKey(validator.FieldName) ? obj.EntityData[validator.FieldName] : null);
                if (validator.Validate(val)) continue;

                valid = false;
                if (obj.ModelState.ContainsKey(validator.FieldName))
                {
                    obj.ModelState[validator.FieldName].Errors.Add(new ModelError(validator.Message));
                }
                else
                {
                    var state = new ModelState();
                    state.Errors.Add(new ModelError(validator.Message));
                    obj.ModelState.Add(validator.FieldName, state);
                }
            }

            if (valid)
                obj.ModelState = null;

            return valid;
        }
    }
}
