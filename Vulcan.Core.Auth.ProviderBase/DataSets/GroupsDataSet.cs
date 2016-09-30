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
    public class GroupsDataSet : DataSet
    {
        public GroupsDataSet(DynamicDataContext context) : base(context)
        {
        }
        public Group GetGroupById(long id)
        {
            return (Context.Query<Group>(
                    "[auth]",
                    "[auth_Groups_GetById]",
                    new { id = id },
                    commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public Group GetGroupByName(string name)
        {
            return (Context.Query<Group>(
                    "[auth]",
                    "[auth_Groups_GetByName]",
                    new { name = name },
                    commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public List<Group> GetGroupByAudienceId(string id)
        {
            return (Context.Query<Group>(
                    "[auth]",
                    "[auth_Groups_GetByAudienceId]",
                    new { AudienceId = id },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        /// <exception cref="GroupNotExistsException">Throws when Group not exists.</exception>
        public Group UpdateGroup(long id, Group group)
        {

            var existingGroup = GetGroupById(id);

            if (existingGroup == null) throw new GroupNotExistsException();

            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            Context.ExecuteScalar(
                        "[auth]",
                        "[auth_Groups_Update]",
                        new
                        {
                            Id = id,
                            Name = group.Name,
                            Description = group.Description,
                            AudienceId = group.AudienceId,
                            SystemId = group.IsSystemId,
                            UpdatedByUsername = identity.Name,
                            UpdatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure);

            return group;

        }

        /// <exception cref="GroupNotExistsException">Throws when Group not exists.</exception>
        public async Task DeleteGroupAsync(long id)
        {
            var existingGroup = (Context.Query<Group>(
                "[auth]",
                "[auth_Groups_GetById]",
                new { id = id },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (existingGroup == null) throw new GroupNotExistsException();

            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_Groups_Delete]",
                          new
                          {
                              id = id
                          },
                          commandType: CommandType.StoredProcedure);


        }

        /// <exception cref="GroupNotExistsException">Throws when added Group exists on database.</exception>
        public async Task<Group> AddGroupAsync(Group group)
        {
            if (GetGroupByName(group.Name) != null)
            {
                throw new GroupNotExistsException();
            }

            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            var id =
                (await
                    Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_Groups_Add]",
                        new
                        {
                            Name = group.Name,
                            Description = group.Description,
                            AudienceId = group.AudienceId,
                            CreatedByUsername = identity.Name,
                            CreatedByClientId = identity.ClientId,
                            SystemId = group.IsSystemId,
                        },
                        commandType: CommandType.StoredProcedure));
            group.Id = long.Parse(id.ToString());

            return group;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return (await Context.QueryAsync<Group>(
                    "[auth]",
                    "[auth_Groups_Get]",
                    commandType: CommandType.StoredProcedure)).ToList();
        }


        public async Task<List<Group>> GetGroupsByUserIdAsync(long userSystemIds)
        {
            return (await Context.QueryAsync<Group>(
                    "[auth]",
                    "[auth_Groups_GetByUserSystemId]",
                    new {
                        SystemId = userSystemIds
                    },
                    commandType: CommandType.StoredProcedure)).ToList();
        }




    }
}
