using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class UsernameExistsException : Exception
    {
        public UsernameExistsException() : base("Username already exists.")
        {
        }
    }
}
