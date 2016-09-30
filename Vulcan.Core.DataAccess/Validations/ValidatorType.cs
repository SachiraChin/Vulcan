using System;

namespace Vulcan.Core.DataAccess.Validations
{
    [Flags]
    public enum ValidatorType
    {
        Runtime = 1,
        Database = 2
    }
}
