﻿namespace Vulcan.Core.Auth.AzureAuthProvider.Models
{
    public class AzureToken
    {

        public string access_token { get; set; }
        public int expires_in { get; set; }
        public int expires_on { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public string resource { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }
}
