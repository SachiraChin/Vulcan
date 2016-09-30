using System.Security.Cryptography;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace Vulcan.Core.Auth.Providers
{
    public static class ApiKeyProvider
    {
        public static string GenerateKey(int size = 32)
        {
            var key = new byte[size];
            RNGCryptoServiceProvider.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);

            return base64Secret;
        }
    }
}