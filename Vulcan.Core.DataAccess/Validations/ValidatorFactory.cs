using System;
using Vulcan.Core.DataAccess.Validations.Validators;

namespace Vulcan.Core.DataAccess.Validations
{
    public static class ValidatorFactory
    {
        public static IValidator Get(string identifier)
        {
            var val = identifier.ToUpper();
            switch (val)
            {
                case "21244FE4-D779-47DC-80EC-FA06E3290A46":
                    return new EmailValidator();
                case "EF728C65-89C8-48F9-8762-50E2AE6B465F":
                    return new RangeValidator();
                case "D95C78D4-0818-4171-9D3D-D3DC0804E923":
                    return new RegExValidator();
                case "0BDDD8DD-89D5-479B-9D3F-841678F4D1E4":
                    return new RequiredValidator();
                case "A7C433A1-D724-47FF-B456-229046C1D2CA":
                    return new ChoiceValidator();
                case "82B8620B-3187-4450-B9A2-BAE2BE1D824A":
                    return new NumberValidator();
                case "96F41DFA-FB50-448B-B9AA-B71A52FCFDE7":
                    return new BooleanValidator();
                case "6D4ABB6B-BBDF-4B71-BD16-E39FB9E68E59":
                    return new DateTimeValidator();
            }

            return null;
        }

        public static Type GetType(string identifier)
        {
            var val = identifier.ToUpper();
            switch (val)
            {
                case "21244FE4-D779-47DC-80EC-FA06E3290A46":
                    return typeof(EmailValidator);
                case "EF728C65-89C8-48F9-8762-50E2AE6B465F":
                    return typeof(RangeValidator);
                case "D95C78D4-0818-4171-9D3D-D3DC0804E923":
                    return typeof(RegExValidator);
                case "0BDDD8DD-89D5-479B-9D3F-841678F4D1E4":
                    return typeof(RequiredValidator);
                case "A7C433A1-D724-47FF-B456-229046C1D2CA":
                    return typeof(ChoiceValidator);
                case "82B8620B-3187-4450-B9A2-BAE2BE1D824A":
                    return typeof(NumberValidator);
                case "96F41DFA-FB50-448B-B9AA-B71A52FCFDE7":
                    return typeof(BooleanValidator);
                case "6D4ABB6B-BBDF-4B71-BD16-E39FB9E68E59":
                    return typeof (DateTimeValidator);
            }

            return null;
        }



    }
}
