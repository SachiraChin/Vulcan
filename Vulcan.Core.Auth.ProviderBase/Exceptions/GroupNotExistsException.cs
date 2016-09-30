using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class GroupNotExistsException : Exception
    {
        public GroupNotExistsException() : base("Group not exists")
        {
        }
    }
}
