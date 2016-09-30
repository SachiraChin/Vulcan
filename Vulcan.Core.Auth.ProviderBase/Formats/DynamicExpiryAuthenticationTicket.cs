using System;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Vulcan.Core.Auth.Formats
{
    public class DynamicExpiryAuthenticationTicket : AuthenticationTicket
    {
        public DynamicExpiryAuthenticationTicket(ClaimsIdentity identity, AuthenticationProperties properties, OAuthAuthorizationServerOptions options, int? expirySpan) : base(identity, properties)
        {
            if (expirySpan.HasValue)
            {
                options.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(expirySpan.Value);
            }
        }
    }
}
