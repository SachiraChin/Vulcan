using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth
{
    public class AuthenticationProviderManager
    {
        private static readonly Dictionary<string, IAuthenticationProvider> Providers = new Dictionary<string, IAuthenticationProvider>(); 
        public static void Register(IAuthenticationProvider provider)
        {
            Providers.Add(provider.GrantName, provider);
        }

        public static Task GrantAccessToken(OAuthGrantCustomExtensionContext context, Func<Task> defaultCallback)
        {
            return Providers.ContainsKey(context.GrantType) ? Providers[context.GrantType].GrantAccessToken(context) : defaultCallback();
        }

        public static Task GrantRefreshToken(string grant, OAuthGrantRefreshTokenContext context, Func<Task> defaultCallback)
        {
            return Providers.ContainsKey(grant) && !Providers[grant].UseGrantRefreshTokenDefault ? Providers[grant].GrantRefreshToken(context) : defaultCallback();
        }

        public static Task SyncUsers(TenantUserIdentity currentIdentity, ApiUserInternal currentUser)
        {
            return Providers.ContainsKey(currentUser.ExternalProviderName) ? Providers[currentUser.ExternalProviderName].SyncUsers(currentIdentity, currentUser) : null;
        }


    }
}
