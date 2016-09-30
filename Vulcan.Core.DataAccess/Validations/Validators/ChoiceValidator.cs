using System;
using System.Collections.Generic;
using System.Linq;
using Vulcan.Core.DataAccess.Constraints;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.DataAccess.Validations.Validators
{
    public class ChoiceValidator : IValidator
    {

        public Guid Id => new Guid("A7C433A1-D724-47FF-B456-229046C1D2CA");
        public ValidatorType Type => ValidatorType.Runtime;

        public int ValidationId { get; set; }

        public string FieldName { get; set; }

        public string Message { get; set; }

        public List<string> Choices { get; set; }


        public void DecodeData(string data)
        {
            if (data == null) return;

            Choices = data.Split('.').Select(d => d.DecodeFromBase64()).ToList();
        }

        public string EncodeData()
        {
            return string.Join(".", Choices.Select(d => d.EncodeToBase64()));
        }

        public bool Validate(object value, string data = null)
        {
            if (value == null)
                return true;

            DecodeData(data);

            return Choices.Contains(value.ToString());
        }
        public IEnumerable<IConstraintProvider> GetConstraints()
        {
            return null;
        }
    }
}
