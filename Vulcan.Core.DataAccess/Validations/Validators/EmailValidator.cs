using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vulcan.Core.DataAccess.Constraints;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class EmailValidator : IValidator
    {
        public Guid Id => new Guid("21244FE4-D779-47DC-80EC-FA06E3290A46");
        public ValidatorType Type => ValidatorType.Runtime;

        public int ValidationId { get; set; }

        public string FieldName { get; set; }

        public string Message { get; set; }

        public bool Validate(object value, string data = null)
        {
            if (value == null) return true;

            var valueStr = value.ToString();
            // RegEx source: https://msdn.microsoft.com/en-us/library/01escwtf.aspx
            try
            {
                return Regex.IsMatch(valueStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
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
