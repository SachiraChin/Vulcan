using System;
using Vulcan.Core.DataAccess.Entities;

namespace Vulcan.Core.Auth.Models
{
    public class Audience : DynamicEntity
    {
        public string AudienceId { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }

        public int CreatedByUserId { get; set; }
        public int CreatedByClientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public int UpdatedByClientId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}