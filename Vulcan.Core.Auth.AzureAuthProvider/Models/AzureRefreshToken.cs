using System;

namespace Vulcan.Core.Auth.AzureAuthProvider.Models
{
    public class AzureRefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExpireIn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string ExternalTenantId { get; set; }
    }
}
