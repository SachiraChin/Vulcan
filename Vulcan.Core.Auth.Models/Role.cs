using System;
using System.Runtime.Serialization;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.DataAccess.Entities;

namespace Vulcan.Core.Auth.Models
{
    public class Role : DynamicEntity
    {
        [DataMember]
        public override int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool IsHidden { get; set; }
        [DataMember]
        public RoleType Type { get; set; }
        public int CreatedByUserId { get; set; }
        public int CreatedByClientId { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public int UpdatedByClientId { get; set; }
        [DataMember]
        public DateTime UpdatedDate { get; set; }

        public int AudienceId { get; set; }
    }
}