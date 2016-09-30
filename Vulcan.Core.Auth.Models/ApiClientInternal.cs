using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.DataAccess.Entities;

namespace Vulcan.Core.Auth.Models
{
    public class ApiClientInternal : DynamicEntity
    {
        [MaxLength(32)]
        public string ClientId { get; set; }
        public string ClientSecretHash { get; set; }
        public string ClientSecretSalt { get; set; }
        public ApiClientType Type { get; set; }
        //public string DisplayName { get; set; }
        public int? TokenExpireTimeMinutes { get; set; }
        public int CreatedByUserId { get; set; }
        public int CreatedByClientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public int UpdatedByClientId { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<ApiClientOrigin> ApiClientOrigins { get; set; }
        public long SystemId { get; set; }
        public bool IsSystem { get; set; }
    }
}