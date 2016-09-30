using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class GroupExistsException : Exception
    {
        public GroupExistsException() : base("Group exists")
        {
        }
    }
}
