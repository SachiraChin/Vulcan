using System.Collections.Generic;

namespace Vulcan.Core.Auth.AzureAuthProvider.Models
{
    public class AzureJwtPayload
    {
        public string aud { get; set; }
        public string iss { get; set; }
        public int iat { get; set; }
        public int nbf { get; set; }
        public int exp { get; set; }
        public string ver { get; set; }
        public string tid { get; set; }
        public string oid { get; set; }
        public string upn { get; set; }
        public string sub { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string name { get; set; }
        public List<string> amr { get; set; }
        public string unique_name { get; set; }
    }
}
