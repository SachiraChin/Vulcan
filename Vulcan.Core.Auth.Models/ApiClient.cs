using System.ComponentModel.DataAnnotations;
using Vulcan.Core.Auth.Models.Enums;

namespace Vulcan.Core.Auth.Models
{
    public class ApiClient
    {
        public long Id { get; set; }
        [MaxLength(32)]
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ApiClientType Type { get; set; }
        //public string DisplayName { get; set; }
        public int? TokenExpireTimeMinutes { get; set; }
    }
}