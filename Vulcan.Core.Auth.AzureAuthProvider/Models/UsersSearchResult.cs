using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vulcan.Core.Auth.AzureAuthProvider.Models
{
    class UsersSearchResult
    {
        public List<SearchUser> value { get; set; }
        [JsonProperty("odata.nextLink")]
        public string NextLink { get; set; }
    }

    public class SearchUser
    {
        public string objectType { get; set; }
        public string objectId { get; set; }
        public string displayName { get; set; }
        public string mailNickname { get; set; }
        public List<string> otherMails { get; set; }
        public string userPrincipalName { get; set; }
        public string userType { get; set; }
    }
}
