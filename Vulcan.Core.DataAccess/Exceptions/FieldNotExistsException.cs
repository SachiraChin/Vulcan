using System;

namespace Vulcan.Core.DataAccess.Exceptions
{
    /// <summary>
    /// Throws when field not exists
    /// </summary>
    public class FieldNotExistsException : Exception
    {
        public FieldNotExistsException() : base("Field not exists")
        {
        }
    }
}
