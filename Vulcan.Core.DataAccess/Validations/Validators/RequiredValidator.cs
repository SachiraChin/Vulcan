using System;
using System.Collections.Generic;
using Vulcan.Core.DataAccess.Constraints;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class RequiredValidator : IValidator
    {
        public Guid Id => new Guid("0BDDD8DD-89D5-479B-9D3F-841678F4D1E4");
        public ValidatorType Type => ValidatorType.Runtime;

        public int ValidationId { get; set; }

        public string Message { get; set; }

        public string FieldName { get; set; }

        public bool Validate(object value, string data = null)
        {
            return value?.ToString().Trim() != "";
        }

        public string EncodeData()
        {
            return string.Empty;
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
