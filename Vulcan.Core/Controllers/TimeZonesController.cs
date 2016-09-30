using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Controllers
{
    [RoutePrefix("v1/timezones")]
    public class TimeZonesController:ApiController
    {

        [HttpGet]
        [ResponseType(typeof(List<InternationalTimeZone>))]
        [Route]
        public async Task<IHttpActionResult> GetTimeZoneAsync()
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains("TimeZones"))
                return Ok( (List<InternationalTimeZone>)cache["TimeZones"]);

            var identity = new TenantUserIdentity(User.Identity);
            using (var context=new SystemDataContext(identity.TenantId))
            {
                var timezone = await context.Timezones.GetTimezone();
                var zoneUtc = await context.TimezoneUTCs.GetTimezoneUTC();

                foreach (var zone in timezone)
                {
                    zone.UTCs = zoneUtc.Where(u => u.TimezoneId == zone.Id).ToList();
                }

                CacheItemPolicy policy = new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddMonths(2)};
                cache.Add("TimeZones" , timezone, policy);

                return Ok( timezone);
            }
        }


    }
}