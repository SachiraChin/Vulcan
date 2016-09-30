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
    public static class ApiUserExtensions
    {

        public static DynamicExpiryAuthenticationTicket GetTicket(this ApiUserInternal apiUser, SystemDataContext dbContext, IOwinContext context, 
            OAuthAuthorizationServerOptions options, string grant)
        {

            var identity = new ClaimsIdentity("JWT");
            var tenantId = context.Get<string>("as:tenantId");
            var audienceId = context.Get<string>("as:audienceId");

            identity.AddClaim(new Claim("tid", tenantId));
            identity.AddClaim(new Claim("type", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Name, apiUser.Username));
            identity.AddClaim(new Claim("sub", apiUser.Username));
            identity.AddClaim(new Claim("uid", apiUser.SystemId.ToString()));

            var roles =
                (from role in dbContext.Roles.GetRolesByUsernameAudienceId(apiUser.Username, audienceId)
                 select role.Name).ToList();

            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                            { "audience", audienceId },
                            {"tenant", tenantId},
                            { "subject", apiUser.Username },
                            { "grant", grant },
                            { "expires", apiUser.TokenExpireTimeMinutes?.ToString() ?? "" }
                        });

            // var ticket = new AuthenticationTicket(identity, props);
            var ticket = new DynamicExpiryAuthenticationTicket(identity, props, options, apiUser.TokenExpireTimeMinutes);
            return ticket;
        }
    }
}
