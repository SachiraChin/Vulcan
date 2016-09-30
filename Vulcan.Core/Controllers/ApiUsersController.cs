using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vulcan.Core.Auth;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Exceptions;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Controllers
{
    [Authorize(Roles = "sa")]
    [RoutePrefix("v1/users")]
    public class ApiUsersController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(ApiUserInternal))]
        [Route("{id}")]
        public IHttpActionResult GetById(long id)
        {
            var identity = new TenantUserIdentity(User.Identity);

            if (identity.UserId <= 0)
                return Unauthorized();

            using (var context = new SystemDataContext(identity.TenantId))
            {
                var user = context.ApiUsers.GetUserBySystemId(id);
                return Ok(user);
            }
        }
        [HttpGet]
        [ResponseType(typeof(List<ApiUserInternal>))]
        [Route]
        public async Task<IHttpActionResult> SearchUsersAsync(string searchText)
        {
            var identity = new TenantUserIdentity(User.Identity);

            if (identity.UserId <= 0)
                return Unauthorized();

            using (var context = new SystemDataContext(identity.TenantId))
            {
                var users = await context.ApiUsers.SearchUsers(searchText);
                return Ok(users);
            }
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("sync")]
        public async Task<IHttpActionResult> SyncUsersAsync()
        {
            var identity = new TenantUserIdentity(User.Identity);

            if (identity.UserId == null || identity.UserId <= 0)
                return Unauthorized();

            using (var context = new SystemDataContext(identity.TenantId))
            {
                var currentUser = context.ApiUsers.GetUserBySystemId(identity.UserId.Value);
                await AuthenticationProviderManager.SyncUsers(identity, currentUser);

                return Ok();
            }
        }




        /// <exception cref="UsernameExistsException">Throws when added username exists on database.</exception>
        [HttpPost]
        [ResponseType(typeof(ApiUser))]
        [Route]
        public async Task<IHttpActionResult> PostApiUserAsync(ApiUser entity)
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
                var user = await context.ApiUsers.AddApiUserAsync(entity);

                return Ok(user);
            }
        }


        /// <exception cref="ApiUserNotExistsException">Throws when ApiUser not exists in database.</exception>
        /// <exception cref="UsernameExistsException">Throws when updated username exists in database.</exception>
        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateApiUserAsync(long id, ApiUser entity)
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
                //var roles = await context.Roles.GetRolesByUserSystemIdAsync(id);
                //if (roles.Any(r => r.Type == RoleType.System))
                //{
                //    return Unauthorized();
                //}

                var updatedUser = await context.ApiUsers.UpdateApiUserAsync(id, entity);
                return Ok(updatedUser);
            }
        }

        /// <exception cref="ApiUserNotExistsException">Throws when ApiUser not exists.</exception>
        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteApiUserAsync(long id)
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

                await context.ApiUsers.DeleteApiUserAsync(id);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("{id}/roles")]
        public async Task<IHttpActionResult> AddUserRolesAsync(long id, List<Role> roles)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                var dbRoles = await context.Roles.GetRolesByIdsAsync(roles.Select(r => r.Id).ToList());
                if (roles.Count != dbRoles.Count || dbRoles.Any(r => r.Type == RoleType.ApiClient))
                {
                    return BadRequest();
                }

                if (!User.IsInRole("sa") && dbRoles.Any(r => r.Type == RoleType.System))
                {
                    return Unauthorized();
                }

                var userRoles = await context.Roles.GetRolesByUserSystemIdAsync(id);
                if (userRoles.Any(ur => roles.Any(r => r.Id == ur.Id)))
                {
                    return BadRequest();
                }

                await context.Roles.AddRolesToUserAsync(id, roles.Select(r => r.Id).ToList());

            }
            return Ok();
        }

        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("{id}/roles")]
        public async Task<IHttpActionResult> DeleteUserRolesAsync(int id, List<Role> roles)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                var dbRoles = await context.Roles.GetRolesByIdsAsync(roles.Select(r => r.Id).ToList());
                if (roles.Count != dbRoles.Count)
                {
                    return BadRequest();
                }

                if (!User.IsInRole("sa") && dbRoles.Any(r => r.Type == RoleType.System))
                {
                    return Unauthorized();
                }

                await context.Roles.DeleteRolesToUserAsync(id, roles.Select(r => r.Id).ToList());

            }
            return Ok();
        }


        [HttpGet]
        [ResponseType(typeof(void))]
        [Route("{id}/groups")]
        public async Task<IHttpActionResult> GetApiUserGroupsAsync(long id)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                return Ok(await context.Groups.GetGroupsByUserIdAsync(id));
            }
        }

    }
}
