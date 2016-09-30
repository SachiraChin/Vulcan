using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Vulcan.Core.Auth;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Extensions;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.Auth.Providers;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Get audience from parameters
            var audienceId = context.Parameters["audience"];
            var tenantId = context.Parameters["tenant_id"];

            // Validate audience with database
            if (tenantId != null)
            {
                try
                {
                    using (var dbcontext = new SystemDataContext(tenantId))
                    {
                        var audience = dbcontext.Audiences.GetAudienceByAudinceId(audienceId);
                        if (audience == null)
                        {
                            context.SetError("invalid_audience", $"Invalid audience '{audienceId}'");
                            return Task.FromResult<object>(null);
                        }
                    }
                }
                catch (Exception)
                {
                    context.SetError("invalid_tenant_id", $"Invalid tenant '{tenantId}'");
                    return Task.FromResult<object>(null);
                }
            }

            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            context.OwinContext.Set("as:audienceId", audienceId);
            context.OwinContext.Set("as:clientSecret", clientSecret);
            context.OwinContext.Set("as:tenantId", tenantId);

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            return AuthenticationProviderManager.GrantAccessToken(context, () =>
            {
                if (context.GrantType == "secret")
                {
                    var tenantId = context.OwinContext.Get<string>("as:tenantId");
                    using (var dbcontext = new SystemDataContext(tenantId))
                    {
                        var client =
                            dbcontext.ApiClients.GetApiClientByClientId(context.ClientId, true);

                        if (client == null)
                        {
                            context.SetError("invalid_grant", "Invalid client id or client secret");
                            return Task.FromResult<object>(null);
                        }

                        var origin = context.Request.GetOriginUrl();

                        switch (client.Type)
                        {
                            case ApiClientType.PureClient:
                                var agent = context.Request.Headers["User-Agent"];

                                if (origin == null || agent == null || client.ApiClientOrigins == null)
                                {
                                    context.SetError("invalid_clientType", "Invalid client id");
                                    break;
                                }

                                if (client.ApiClientOrigins.Any(a => a.Origin == origin))
                                {
                                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { origin });
                                    context.Validated(client.GetTicket(dbcontext, context.OwinContext, context.Options, "secret", origin));
                                }
                                else
                                    context.SetError("invalid_clientType", "Invalid client id");
                                break;

                            case ApiClientType.Native:
                                var secret = context.OwinContext.Get<string>("as:clientSecret");
                                if (!string.IsNullOrEmpty(secret) && PasswordHashProvider.ValidatePassword(secret,
                                    new HashObject
                                    {
                                        Hash = client.ClientSecretHash,
                                        Salt = client.ClientSecretSalt
                                    }))
                                    context.Validated(client.GetTicket(dbcontext, context.OwinContext, context.Options, "secret", origin));
                                else
                                    context.SetError("invalid_grant", "Invalid client id or client secret");
                                break;
                            case ApiClientType.System:
                                if (origin != null)
                                {
                                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { origin });
                                }
                                context.Validated(client.GetTicket(dbcontext, context.OwinContext, context.Options, "secret", origin));
                                break;
                            default:
                                context.SetError("invalid_grant", "Invalid client id or client secret");
                                break;
                        }

                        return Task.FromResult<object>(null);

                    }
                }

                return base.GrantCustomExtension(context);
            });
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var tenantId = context.OwinContext.Get<string>("as:tenantId");
            using (var dbcontext = new SystemDataContext(tenantId))
            {
                var user = dbcontext.ApiUsers.GetUserByUsername(context.UserName);
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect");
                    return Task.FromResult<object>(null);
                }

                if (PasswordHashProvider.ValidatePassword(context.Password,
                    new HashObject() { Hash = user.PasswordHash, Salt = user.PasswordSalt }))
                {
                    var ticket = user.GetTicket(dbcontext, context.OwinContext, context.Options, "password");

                    context.Validated(ticket);
                    return Task.FromResult<object>(null);

                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect");
                    return Task.FromResult<object>(null);
                }
            }

        }


        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalaudience = context.Ticket.Properties.Dictionary["audience"];
            var currentClient = context.OwinContext.Get<string>("as:audienceId");

            if (originalaudience != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different audience.");
                return Task.FromResult<object>(null);
            }

            var grant = context.Ticket.Properties.Dictionary["grant"];
            if (grant != null)
            {
                return AuthenticationProviderManager.GrantRefreshToken(grant, context, () =>
                {
                    // On custom refresh token implementation not available
                    // Change auth ticket for refresh token requests
                    var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
                    var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
                    context.Validated(newTicket);

                    return Task.FromResult<object>(null);
                });
            }
            else
            {
                context.SetError("invalid_grant");
                return Task.FromResult<object>(null);
            }

        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            return base.TokenEndpointResponse(context);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            return base.TokenEndpoint(context);
        }

        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            return base.MatchEndpoint(context);
        }

        public override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            return base.AuthorizeEndpoint(context);
        }

        public override Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {
            return base.AuthorizationEndpointResponse(context);
        }

        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            return base.GrantClientCredentials(context);
        }

        public override Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            return base.GrantAuthorizationCode(context);
        }

    }
}