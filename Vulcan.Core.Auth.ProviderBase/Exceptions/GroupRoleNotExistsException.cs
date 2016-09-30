using System;

namespace Vulcan.Core.Auth.Exceptions
{
    public class GroupRoleNotExistsException : Exception
    {
        public GroupRoleNotExistsException() : base("GroupRole not exists")
        {
        }
    }
}
