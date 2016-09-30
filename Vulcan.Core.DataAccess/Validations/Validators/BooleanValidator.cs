using System;
using System.Collections.Generic;
using Vulcan.Core.DataAccess.Constraints;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class BooleanValidator : IValidator
    {
        public Guid Id  => new Guid("96F41DFA-FB50-448B-B9AA-B71A52FCFDE7");
        public ValidatorType Type => ValidatorType.Runtime;
        public int ValidationId { get; set; }
        public string FieldName { get; set; }
        public string Message { get; set; }
        public bool Validate(object value, string data = null)
        {
            return value is bool;
        }

        public string EncodeData()
        {
            return null;
        }

        public void DecodeData(string data)
        {

        }
        public IEnumerable<IConstraintProvider> GetConstraints()
        {
            return null;
        }
    }
}
