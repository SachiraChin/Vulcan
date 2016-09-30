using System;
using Newtonsoft.Json;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.DataAccess.Entities;

namespace Vulcan.Core.Auth.Models
{
    public class ApiUserInternal : DynamicEntity
    {
        [JsonProperty]
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int? TokenExpireTimeMinutes { get; set; }
        [JsonProperty]
        public int CreatedByUserId { get; set; }
        [JsonProperty]
        public int CreatedByClientId { get; set; }
        [JsonProperty]
        public DateTime CreatedDate { get; set; }
        [JsonProperty]
        public int UpdatedByUserId { get; set; }
        [JsonProperty]
        public int UpdatedByClientId { get; set; }
        [JsonProperty]
        public DateTime UpdatedDate { get; set; }
        [JsonProperty]
        public ApiUserType Type { get; set; } = ApiUserType.Internal;
        public int? ExternalRefId { get; set; }
        public string ExternalProviderName { get; set; }
        [JsonProperty("Id")]
        public long SystemId { get; set; }
        [JsonProperty]
        public string FirstName { get; set; }
        [JsonProperty]
        public string LastName { get; set; }
        [JsonProperty]
        public string DisplayName { get; set; }
    }
}