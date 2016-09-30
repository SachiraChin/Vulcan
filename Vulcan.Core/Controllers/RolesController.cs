using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Controllers
{
    [Authorize]
    [RoutePrefix("v1/roles")]
    public class RolesController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(List<Role>))]
        [Route]
        public async Task<IHttpActionResult> GetRolesAsync()
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                //var userInfo = User.ExtractClaimPrinciple();
                List<Role> userRoles = null;
                if (identity.UserId != null && identity.IdentityType == "User")
                {
                    userRoles = await context.Roles.GetRolesByUserSystemIdAsync(identity.UserId.Value);
                }
                if (identity.ClientId != null && identity.IdentityType == "Application")
                {
                    userRoles = await context.Roles.GetRolesByClientSystemIdAsync(identity.ClientId.Value);
                }

                if (userRoles != null && userRoles.Any(r => r.Type == RoleType.System))
                {
                    return Ok(await context.Roles.GetRolesAsync(true));
                }

                return Ok(await context.Roles.GetRolesAsync(false));
            }
        }

        [HttpPost]
        [ResponseType(typeof(Role))]
        [Route]
        public async Task<IHttpActionResult> PostApiUserAsync(Role entity)
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
                //var userInfo = User.ExtractClaimPrinciple();
                List<Role> userRoles = null;
                if (identity.UserId != null && identity.IdentityType == "User")
                {
                    userRoles = await context.Roles.GetRolesByUserSystemIdAsync(identity.UserId.Value);
                }
                if (identity.ClientId != null && identity.IdentityType == "Application")
                {
                    userRoles = await context.Roles.GetRolesByClientSystemIdAsync(identity.ClientId.Value);
                }

                if (entity.Type == RoleType.System && (userRoles == null || userRoles.All(r => r.Type != RoleType.System)))
                {
                    return Unauthorized();
                }

                var role = await context.Roles.AddRoleAsync(entity);

                return Ok(role);
            }
        }
    }
}
