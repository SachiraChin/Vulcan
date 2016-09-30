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
    public class GroupRolesDataSet : DataSet
    {
        public GroupRolesDataSet(DynamicDataContext context) : base(context)
        {
        }

        public async Task AddGroupRoleAsync(long groupId, List<int> roles)
        {

            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            foreach (var i in roles) dt.Rows.Add(i);

            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);



           await Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_GroupRoles_Add]",
                        new
                        {
                            GroupId = groupId,
                            Ids = dt,
                            CreatedByUsername = identity.Name,
                            CreatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure);
        }

        /// <exception cref="GroupRoleNotExistsException">Throws when Group not exists.</exception>
        public async Task DeleteGroupRoleAsync(long groupid, List<int> roles)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            roles.ForEach(i => dt.Rows.Add(i));

            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_GroupRoles_Delete]",
                          new
                          {
                              groupid = groupid,
                              Ids = dt
                          },
                          commandType: CommandType.StoredProcedure);

        }

        public async Task DeleteGroupRoleByGroupIdAsync(long groupid)
        {

            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_GroupRoles_DeleteByGroupId]",
                          new
                          {
                              groupid = groupid,
                          },
                          commandType: CommandType.StoredProcedure);

        }

        /// <exception cref="GroupNotExistsException">Throws when Group not exists.</exception>
        public async Task UpdateGroupRoleAsync(long groupid, List<int> roles)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            foreach (var i in roles) dt.Rows.Add(i);

            var existingGroupRole = (Context.Query<GroupRole>(
                "[auth]",
                "[auth_GroupRoles_GetByGroupId]",
                new { groupid = groupid },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (existingGroupRole == null) throw new GroupNotExistsException();

            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            await Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_GroupRoles_Update]",
                        new
                        {
                            GroupId = groupid,
                            Ids = dt,
                            UpdatedByUsername = identity.Name,
                            UpdatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure);

        }

    }
}
