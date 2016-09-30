using System;
using System.Collections.Generic;
using Vulcan.Core.DataAccess.Constraints;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class RangeValidator : IValidator
    {
        public enum RangeValidatorType
        {
            MinMaxIncluded,
            MinMaxExcluded,
            MinIncludedMaxExcluded,
            MinExcludedMaxIncluded
        }

        public Guid Id => new Guid("EF728C65-89C8-48F9-8762-50E2AE6B465F");
        public ValidatorType Type => ValidatorType.Runtime;

        public int ValidationId { get; set; }

        public string Message { get; set; }

        public string FieldName { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public RangeValidatorType Mode { get; set; }

        public bool Validate(object value, string data = null)
        {
            if (value == null) return true;

            double valueDouble;
            if (double.TryParse(value.ToString(), out valueDouble))
            {
                switch (Mode)
                {
                    case RangeValidatorType.MinMaxIncluded:
                        return valueDouble >= MinValue && valueDouble <= MaxValue;
                    case RangeValidatorType.MinMaxExcluded:
                        return valueDouble > MinValue && valueDouble < MaxValue;
                    case RangeValidatorType.MinIncludedMaxExcluded:
                        return valueDouble >= MinValue && valueDouble < MaxValue;
                    case RangeValidatorType.MinExcludedMaxIncluded:
                        return valueDouble > MinValue && valueDouble <= MaxValue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                return false;
            }
        }

        public string EncodeData()
        {
            var data = MinValue + "|" + MaxValue + "|" + Mode;
            return data;
        }

        public void DecodeData(string data)
        {
            if (data == null) return;

            var parts = data.Split('|');
            if (parts.Length < 3) return;

            MinValue = parts[0].ToDouble();
            MaxValue = parts[1].ToDouble();

            RangeValidatorType type;
            Enum.TryParse(parts[2], false, out type);
            Mode = type;
        }
        public IEnumerable<IConstraintProvider> GetConstraints()
        {
            return null;
        }
    }
}
