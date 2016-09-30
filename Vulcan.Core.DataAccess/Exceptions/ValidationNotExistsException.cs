using System;

namespace Vulcan.Core.DataAccess.Exceptions
{
    public class ValidationNotExistsException : Exception
    {
        public ValidationNotExistsException() : base("Validation not exists")
        {
        }
    }
}
