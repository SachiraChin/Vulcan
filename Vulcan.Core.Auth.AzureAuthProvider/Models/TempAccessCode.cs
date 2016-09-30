using System;

namespace Vulcan.Core.Auth.AzureAuthProvider.Models
{
    public class TempAccessCode
    {
        public long ApiUserId { get; set; }
        public string AccessCode { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsActive { get; set; }
    }
}
