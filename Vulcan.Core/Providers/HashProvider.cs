using System.Security.Cryptography;
using System.Text;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace Vulcan.Core.Providers
{
    public static class HashProvider
    {
        public static string CreateHash(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            var base64Secret = TextEncodings.Base64Url.Encode(hash);
            return base64Secret;
        }
    }
}