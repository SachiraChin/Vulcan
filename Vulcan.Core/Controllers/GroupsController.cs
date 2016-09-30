using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Exceptions;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Controllers
{
    [Authorize(Roles = "sa")]
    [RoutePrefix("v1/groups")]
    public class GroupsController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(Group))]
        [Route]
        public async Task<IHttpActionResult> PostGroupAsync(Group group)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }

            using (var context = new SystemDataContext(identity.TenantId))
            {
                var grp = await context.Groups.AddGroupAsync(group);
                await context.GroupRoles.AddGroupRoleAsync(grp.Id, group.SelectedRoles.Select(r => r.Id).ToList());
                await context.GroupUsers.AddGroupUserAsync(grp.Id, group.SelectedUsers.Select(r => r.SystemId).ToList());
                return Ok(grp);
            }
        }

        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateGroup(long id, Group entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != entity.Id)
                return BadRequest();

            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                var updatedGroup = context.Groups.UpdateGroup(id, entity);
                await context.GroupRoles.DeleteGroupRoleByGroupIdAsync(id);
                await context.GroupUsers.DeleteGroupUserByGroupIdAsync(id);
                await context.GroupRoles.AddGroupRoleAsync(id, entity.SelectedRoles.Select(r => r.Id).ToList());
                await context.GroupUsers.AddGroupUserAsync(id, entity.SelectedUsers.Select(r => r.SystemId).ToList());

                return Ok(updatedGroup);
            }
        }

        [HttpGet]
        [ResponseType(typeof(List<Group>))]
        [Route]
        public async Task<IHttpActionResult> GetGroupsAsync()
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                var user = await context.Groups.GetGroupsAsync();

                return Ok(user);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Group))]
        [Route("{id}")]
        public IHttpActionResult GetGroupById(long id)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }

            using (var context = new SystemDataContext(identity.TenantId))
            {
                var group = context.Groups.GetGroupById(id);
                group.SelectedRoles = context.Roles.GetRolesByGroupId(id);
                group.SelectedUsers = context.ApiUsers.GetApiUsersByGroupId(id);

                return Ok(group);
            }

        }

        /// <exception cref="GroupNotExistsException">Throws when ApiUser not exists.</exception>
        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteGroupAsync(long id)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            //var userInfo = User.ExtractClaimPrinciple();
            if (identity.UserId == id)
            {
                return BadRequest();
            }

            using (var context = new SystemDataContext(identity.TenantId))
            {
                var roles = await context.Roles.GetRolesByUserSystemIdAsync(id);
                if (!User.IsInRole("sa") && roles.Any(r => r.Type == RoleType.System))
                {
                    return Unauthorized();
                }

                await context.Groups.DeleteGroupAsync(id);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}