using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.DataSets
{
    public class RolesDataSet : DataSet
    {
        public RolesDataSet(DynamicDataContext context) : base(context)
        {
        }
        public List<Role> GetRolesByGroupId(long groupid)
        {
            return (Context.Query<Role>(
                    "[auth]",
                    "[auth_Roles_GetByGroupId]",
                    new { groupid = groupid },
                    commandType: CommandType.StoredProcedure)).ToList();
        }
        public List<Role> GetRolesByClientId(string clientId)
        {
            return (Context.Query<Role>(
                    "[auth]",
                    "[auth_Roles_GetByClientId]",
                    new { clientId = clientId },
                    commandType: CommandType.StoredProcedure)).ToList();
        }


        public List<Role> GetRolesByUsernameAudienceId(string username, string audienceId)
        {
            return (Context.Query<Role>(
                    "[auth]",
                    "[auth_Roles_GetByUsernameAudienceId]",
                    new
                    {
                        Username = username,
                        AudienceId = audienceId
                    },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public List<Role> GetRolesByUsername(string username)
        {
            return (Context.Query<Role>(
                    "[auth]",
                    "[auth_Roles_GetByUsername]",
                    new { username = username },
                    commandType: CommandType.StoredProcedure)).ToList();
        }
        
        public async Task<List<Role>> GetRolesByIdsAsync(List<int> ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof (int));
            ids.ForEach(i => dt.Rows.Add(i));
            return (await Context.QueryAsync<Role>(
                    "[auth]",
                    "[auth_Roles_GetByIdList]",
                    new { Ids = dt },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task AddRolesToUserAsync(long userSystemId, List<int> roles)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            foreach (var i in roles) dt.Rows.Add(i);

            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            await Context.ExecuteAsync(
                "[auth]",
                "[auth_Roles_AddApiUserRoles]",
                new
                {
                    userSystemId = userSystemId,
                    Ids = dt,
                    CreatedByUsername = identity.Name,
                    CreatedByClientId = identity.ClientId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddRoleToUserAsync(long userSystemId, int role)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            dt.Rows.Add(role);
            
            await Context.ExecuteAsync(
                "[auth]",
                "[auth_Roles_AddApiUserRoles]",
                new
                {
                    userSystemId = userSystemId,
                    Ids = dt
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteRolesToUserAsync(long userId, List<int> roles)
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            roles.ForEach(i => dt.Rows.Add(i));

            await Context.ExecuteAsync(
                "[auth]",
                "[auth_Roles_DeleteApiUserRoles]",
                new
                {
                    userId = userId,
                    Ids = dt
                },
                commandType: CommandType.StoredProcedure);
        }
        
        public async Task<List<Role>> GetRolesByUserSystemIdAsync(long userSystemId)
        {
            return (await Context.QueryAsync<Role>(
                    "[auth]",
                    "[auth_Roles_GetByUserSystemId]",
                    new { userSystemId = userSystemId },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<Role>> GetRolesByClientSystemIdAsync(long clientSystemId)
        {
            return (await Context.QueryAsync<Role>(
                    "[auth]",
                    "[auth].[auth_Roles_GetByClientSystemId]",
                    new { clientSystemId = clientSystemId },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<Role>> GetRolesAsync(bool isSystem)
        {
            return (await Context.QueryAsync<Role>(
                    "[auth]",
                    "[auth_Roles_Get]",
                    new { isSystem = isSystem },
                    commandType: CommandType.StoredProcedure)).ToList();
        }


        public async Task<Role> AddRoleAsync(Role role)
        {
            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            var id =
                (await
                    Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_Roles_Add]",
                        new
                        {
                            Name = role.Name,
                            Title = role.Title,
                            IsHidden = role.IsHidden,
                            Type = role.Type,
                            CreatedByUsername = identity.Name,
                            CreatedByClientId = identity.ClientId,
                            AudienceId = role.AudienceId
                        },
                        commandType: CommandType.StoredProcedure));
            role.Id = int.Parse(id.ToString());

            return role;
        }
    }
}