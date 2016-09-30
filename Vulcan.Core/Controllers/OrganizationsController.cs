using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Controllers
{
    [Authorize(Roles = "sa")]
    [RoutePrefix("v1/organizations")]
    public class OrganizationsController:ApiController
    {
        [HttpPost]
        [ResponseType(typeof(Organization))]
        [Route]
        public async Task<IHttpActionResult> PostOrganizationAsync (Organization entity)
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
            using (var context=new SystemDataContext(identity.TenantId))
            {
                var organization = await context.Organizations.AddOrganizationAsync(entity);
                return Ok(organization);
            }
        }

        [HttpPut]
        [ResponseType(typeof(void))]
        [Route]
        public async Task<IHttpActionResult> UpdateOrganization(Organization entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
          
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                var updatedOrganization = await context.Organizations.UpdateOrganizationAsync(entity);
                return Ok(updatedOrganization);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Organization))]
        [Route]
        public async Task<IHttpActionResult> GetOrganizationAsync()
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                Organization organization = await context.Organizations.GetOrganizationAsync();
                return Ok(organization);
            }
        }

    }
}