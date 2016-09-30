using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth
{
    public interface IAuthenticationProvider
    {
        string GrantName { get; }
        bool UseGrantRefreshTokenDefault { get; }
        Task GrantAccessToken(OAuthGrantCustomExtensionContext context);
        Task GrantRefreshToken(OAuthGrantRefreshTokenContext context);
        Task SyncUsers(TenantUserIdentity currentIdentity, ApiUserInternal currentUser);
    }
}
