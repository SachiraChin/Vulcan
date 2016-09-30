using System.Security.Claims;
using System.Security.Principal;

namespace Vulcan.Core.Utilities
{
    public class TenantUserIdentity : ClaimsIdentity
    {
        public string TenantId { get; set; }
        public long? ClientId { get; set; }
        public long? UserId { get; set; }
        public string IdentityType { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public new string Name { get; set; }

        public TenantUserIdentity(IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null) return;

            TenantId = claimsIdentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
            var cid = claimsIdentity.FindFirst("cid")?.Value;
            if (cid != null)
                ClientId = long.Parse(cid);

            var uid = claimsIdentity.FindFirst("uid")?.Value;
            if (uid != null)
            {
                UserId = long.Parse(uid);
            }
            IdentityType = claimsIdentity.FindFirst("type")?.Value;
            Issuer = claimsIdentity.FindFirst("iss")?.Value;
            Audience = claimsIdentity.FindFirst("aud")?.Value;
            Name = identity.Name;
        }

        public bool IsValid()
        {
            return TenantId != null && (ClientId != 0 || UserId != 0);
        }
    }
}
