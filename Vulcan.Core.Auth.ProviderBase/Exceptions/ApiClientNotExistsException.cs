using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class ApiClientNotExistsException : Exception
    {
        public ApiClientNotExistsException() : base("ApiClient not exists")
        {
        }
    }
}
