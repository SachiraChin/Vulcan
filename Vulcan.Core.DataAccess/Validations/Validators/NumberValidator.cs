using System;
using System.Collections.Generic;
using Vulcan.Core.DataAccess.Constraints;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class NumberValidator : IValidator
    {
        public Guid Id => new Guid("82B8620B-3187-4450-B9A2-BAE2BE1D824A");
        public ValidatorType Type => ValidatorType.Runtime;
        public int ValidationId { get; set; }
        public string FieldName { get; set; }
        public string Message { get; set; }
        public bool Validate(object value, string data = null)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
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
