using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class GroupUserNotExistsException : Exception
    {
        public GroupUserNotExistsException() : base("GroupUser not exists")
        {
        }
    }
}
