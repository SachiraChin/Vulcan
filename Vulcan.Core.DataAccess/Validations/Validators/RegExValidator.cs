using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vulcan.Core.DataAccess.Constraints;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class RegExValidator : IValidator
    {
        public Guid Id => new Guid("D95C78D4-0818-4171-9D3D-D3DC0804E923");
        public ValidatorType Type => ValidatorType.Runtime;

        public int ValidationId { get; set; }

        public string Message { get; set; }

        public string FieldName { get; set; }

        public string RegularExpression { get; set; }
        public RegexOptions RegexOptions { get; set; }

        public bool Validate(object value, string data = null)
        {
            if (value == null)
                return true;

            DecodeData(data);

            var valueStr = value.ToString();
            var match = Regex.Match(valueStr, RegularExpression, RegexOptions);

            return match.Success;
        }

        public string EncodeData()
        {
            var data = RegularExpression.EncodeToBase64() + "." + RegexOptions.ToString().EncodeToBase64();
            return data;
        }

        public void DecodeData(string data)
        {
            if (data == null) return;

            var parts = data.Split('.');
            if (parts.Length < 2) return;

            RegularExpression = parts[0].DecodeFromBase64();

            RegexOptions regexOptions;
            Enum.TryParse(parts[1].DecodeFromBase64(), false, out regexOptions);
            RegexOptions = regexOptions;
        }
        public IEnumerable<IConstraintProvider> GetConstraints()
        {
            return null;
        }
    }
}
