using System;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Thinktecture.IdentityModel.Tokens;
using Vulcan.Core.Auth.DataContexts;

namespace Vulcan.Core.Formats
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var audienceId = data.Properties.Dictionary.ContainsKey("audience") ? data.Properties.Dictionary["audience"] : null;
            var tenantId = data.Properties.Dictionary.ContainsKey("tenant") ? data.Properties.Dictionary["tenant"] : null;

            if (string.IsNullOrWhiteSpace(audienceId)) throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

            using (var context = new SystemDataContext(tenantId))
            {
                var audience = context.Audiences.GetAudienceByAudinceId(audienceId);

                var symmetricKeyAsBase64 = audience.Secret;

                var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

                var signingKey = new HmacSigningCredentials(keyByteArray);

                var issued = data.Properties.IssuedUtc;
                var expires = data.Properties.ExpiresUtc;
                var expireTime = data.Properties.Dictionary.ContainsKey("expires") ? data.Properties.Dictionary["expires"] : null;
                if (!string.IsNullOrEmpty(expireTime))
                {
                    expires = data.Properties.IssuedUtc.Value.AddMinutes(int.Parse(expireTime));
                }
                var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
                
                var handler = new JwtSecurityTokenHandler();

                var jwt = handler.WriteToken(token);

                return jwt;
            }

        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}