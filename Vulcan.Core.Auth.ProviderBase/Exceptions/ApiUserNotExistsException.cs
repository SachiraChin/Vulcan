using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class ApiUserNotExistsException : Exception
    {
        public ApiUserNotExistsException() : base("ApiUser not exists")
        {
        }
    }
}
