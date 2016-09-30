using System;
using System.Collections.Generic;
using Vulcan.Core.DataAccess.Constraints;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class DateTimeValidator : IValidator
    {
        public Guid Id => new Guid("6D4ABB6B-BBDF-4B71-BD16-E39FB9E68E59");
        public ValidatorType Type => ValidatorType.Runtime;
        public int ValidationId { get; set; }
        public string FieldName { get; set; }
        public string Message { get; set; }
        public bool Validate(object value, string data = null)
        {
            return value is DateTime;
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
