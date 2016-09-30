using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Formats;
using Vulcan.Core.Auth.Models;

namespace Vulcan.Core.Auth.Extensions
{
    public static class ApiClientExtensions
    {
        public static DynamicExpiryAuthenticationTicket GetTicket(this ApiClientInternal apiClient, SystemDataContext dbContext, IOwinContext context, 
            OAuthAuthorizationServerOptions options, string grant, string origin)
        {

            var identity = new ClaimsIdentity("JWT");
            var tenantId = context.Get<string>("as:tenantId");
            identity.AddClaim(new Claim("tid", tenantId));
            identity.AddClaim(new Claim("type", "Application"));
            identity.AddClaim(new Claim(ClaimTypes.Name, apiClient.ClientId));
            identity.AddClaim(new Claim("sub", apiClient.ClientId));
            identity.AddClaim(new Claim("cid", apiClient.SystemId.ToString()));

            if (origin != null)
            {
                identity.AddClaim(new Claim("origin", origin));
            }

            var roles =
                (from role in dbContext.Roles.GetRolesByClientId(apiClient.ClientId)
                 select role.Name).ToList();

            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var audienceId = context.Get<string>("as:audienceId");
            var props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                            {"audience", audienceId},
                            {"tenant", tenantId},
                            {"subject", apiClient.ClientId},
                            {"grant", grant},
                            {"expires", apiClient.TokenExpireTimeMinutes?.ToString() ?? ""}
                        });

            // var ticket = new AuthenticationTicket(identity, props);
            var ticket = new DynamicExpiryAuthenticationTicket(identity, props, options, apiClient.TokenExpireTimeMinutes);
            return ticket;
        }
    }
}
