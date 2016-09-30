using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vulcan.Core.Auth.Exceptions;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.DataSets
{
    public class GroupUsersDataSet : DataSet
    {
        public GroupUsersDataSet(DynamicDataContext context) : base(context)
        {
        }

        public List<GroupUser> GetGroupUserByApiUserId(long apiuserid)
        {
            return (Context.Query<GroupUser>(
                    "[auth]",
                    "[auth_GroupUsers_GetByApiUserId]",
                    new { apiuserid = apiuserid },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public GroupUser GetGroupUserByApiUserIdGroupId(long apiuserid, long groupid)
        {
            return (Context.Query<GroupUser>(
                    "[auth]",
                    "[auth_GroupUsers_GetByApiUserIdGroupId]",
                    new { apiuserid = apiuserid, groupid=groupid },
                    commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task DeleteGroupUserAsync(long groupid, List<long> users)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(long));
            users.ForEach(i => dt.Rows.Add(i));

            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_GroupUsers_Delete]",
                          new
                          {
                              groupid = groupid,
                              Ids = dt
                          },
                          commandType: CommandType.StoredProcedure);

        }

        public async Task DeleteGroupUserByGroupIdAsync(long groupid)
        {

            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_GroupUsers_DeleteByGroupId]",
                          new
                          {
                              groupid = groupid,
                          },
                          commandType: CommandType.StoredProcedure);

        }

        /// <exception cref="GroupNotExistsException">Throws when Group not exists.</exception>
        public async Task UpdateGroupUserAsync(long groupid, List<long> users)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(long));
            foreach (var i in users) dt.Rows.Add(i);

            var existingGroupUser = (Context.Query<GroupUser>(
                "[auth]",
                "[auth_GroupUsers_GetByApiUserIdGroupId]",
                new { groupid = groupid },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (existingGroupUser == null) throw new GroupNotExistsException();

            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            await Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_GroupUsers_Update]",
                        new
                        {
                            GroupId = groupid,
                            Ids = dt,
                            UpdatedByUsername = identity.Name,
                            UpdatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure);

        }

        /// <exception cref="GroupUserNotExistsException">Throws when added Group exists on database.</exception>
        public async Task AddGroupUserAsync(long groupId, List<long> users)
        {
            //if (GetGroupUserByApiUserId(groupId) == null)
            //{
            //    throw new GroupUserNotExistsException();
            //}

            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(long));
            foreach (var i in users) dt.Rows.Add(i);

            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            await Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_GroupUsers_Add]",
                        new
                        {
                            GroupId = groupId,
                            Ids = dt,
                            CreatedByUsername = identity.Name,
                            CreatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure);
        }
    }
}
