using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Formats;
using Vulcan.Core.Providers;

namespace Vulcan.Core
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            var config = new HttpConfiguration();

            WebApiConfig.Register(config);
            ConfigureOAuth(app);
            app.UseWebApi(config);

            // For swashbuckle
            AreaRegistration.RegisterAllAreas();
        }
        public void ConfigureOAuth(IAppBuilder app)
        {

            var issuer = ConfigurationManager.AppSettings["AuthorizationIssuerName"];

            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = false,
                TokenEndpointPath = new PathString("/auth"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["AccessTokenLifeSpanMinutes"])),
                Provider = new OAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(issuer),
                RefreshTokenProvider = new RefreshTokenProvider()
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);


            //var audience = ConfigurationManager.AppSettings["AuthServiceAudienceId"];
            //var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["AuthServiceAudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        AudienceValidator = (audiences, token, parameters) =>
                        {
                            var jwtToken = token as JwtSecurityToken;
                            var tenantId = jwtToken?.Payload["tid"]?.ToString();
                            return tenantId != null && GetTenantAudiences(tenantId).Any(a => audiences.Contains(a.AudienceId));
                        },
                        IssuerSigningKeyResolver = (token, securityToken, identifier, parameters) =>
                        {
                            var jwtToken = securityToken as JwtSecurityToken;
                            var tenantId = (jwtToken?.Payload.ContainsKey("tid") ?? false) ? jwtToken.Payload["tid"]?.ToString() : null;
                            var audienceId = (jwtToken?.Payload.ContainsKey("aud") ?? false) ? jwtToken.Payload["aud"]?.ToString() : null;
                            if (tenantId == null || audienceId == null) return null;
                            
                            var audience = GetTenantAudiences(tenantId).FirstOrDefault(a => a.AudienceId == audienceId);
                            return audience == null ? null : new InMemorySymmetricSecurityKey(TextEncodings.Base64Url.Decode(audience.Secret));
                        },
                        IssuerValidator = (s, token, parameters) =>
                        {
                            return issuer;
                        }
                    }
                });
        }

        private List<Audience> GetTenantAudiences(string tenantId)
        {
            var cacheKey = $"{tenantId}_audiences";
            var tenantAudiences = MemoryCache.Default.Get(cacheKey) as List<Audience>;
            if (tenantAudiences != null) return tenantAudiences;

            using (var context = new SystemDataContext(tenantId))
            {
                tenantAudiences = context.Audiences.GetAudiences();

                var policy = new CacheItemPolicy
                {
                    Priority = CacheItemPriority.Default,
                    SlidingExpiration = TimeSpan.FromHours(24),
                };

                MemoryCache.Default.Set(cacheKey, tenantAudiences, policy);
            }

            return tenantAudiences;
        }
    }
}
