using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Providers;

namespace Vulcan.Core.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var audience = context.Ticket.Properties.Dictionary["audience"];
            var grant = context.Ticket.Properties.Dictionary["grant"];

            if (string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(grant))
            {
                return;
            }

            var refreshTokenId = ApiKeyProvider.GenerateKey();
            var tenantId = context.OwinContext.Get<string>("as:tenantId");

            using (var dbContext = new SystemDataContext(tenantId))
            {
                var subject = context.Ticket.Properties.Dictionary["subject"];

                var refreshToken = new RefreshToken()
                {
                    Subject = subject,
                    IssuedDate = context.Ticket.Properties.IssuedUtc.Value.DateTime,
                    ExpiryDate = context.Ticket.Properties.ExpiresUtc.Value.DateTime,
                    TokenHash = HashProvider.CreateHash(refreshTokenId),
                    Ticket = context.SerializeTicket()
                };

                await dbContext.RefreshTokens.AddTokenAsync(refreshToken);
            }

            context.SetToken(refreshTokenId);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            //var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = HashProvider.CreateHash(context.Token);
            var tenantId = context.OwinContext.Get<string>("as:tenantId");
            using (var dbContext = new SystemDataContext(tenantId))
            {
                var token = await dbContext.RefreshTokens.GetByHashAsync(hashedTokenId);
                if (token != null)
                {
                    context.DeserializeTicket(token.Ticket);
                    await dbContext.RefreshTokens.DeleteByHashAsync(hashedTokenId);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}