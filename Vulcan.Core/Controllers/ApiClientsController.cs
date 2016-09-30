using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Controllers
{
    [RoutePrefix("v1/clients")]
    public class ApiClientsController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ApiClient))]
        [Route]
        public async Task<IHttpActionResult> PostApiClientAsync(ApiClient entity)
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
                var addedClient = await context.ApiClients.AddApiClientAsync(entity);

                return Ok(addedClient);
            }
        }
        
        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateApiClientAsync(long id, [FromBody] ApiClient entity, [FromUri] bool resetClientId = false, [FromUri] bool resetSecret = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
                return BadRequest();

            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                var updatedClient = await context.ApiClients.UpdateApiClientAsync(id, entity, resetClientId, resetSecret);
                return Ok(updatedClient);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteApiClientAsync(long id)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                await context.ApiClients.DeleteApiClientAsync(id);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("{id}/origins")]
        public async Task<IHttpActionResult> PostApiClientOriginAsync(long id, string origin)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                await context.ApiClients.AddOriginAsync(id, origin);
            }
            return Ok();
        }

        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("{id}/origins/{originId}")]
        public async Task<IHttpActionResult> DeleteApiClientOriginAsync(long id, int originId)
        {
            var identity = new TenantUserIdentity(User.Identity);
            if (!identity.IsValid())
            {
                return Unauthorized();
            }
            using (var context = new SystemDataContext(identity.TenantId))
            {
                await context.ApiClients.DeleteApiClientOriginAsync(originId);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }


        protected override void Dispose(bool disposing)
        {
            //context.Dispose();

            base.Dispose(disposing);
        }
    }
}