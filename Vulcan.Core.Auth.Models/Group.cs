using System;
using System.Collections.Generic;

namespace Vulcan.Core.Auth.Models
{
    public class Group
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AudienceId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSystemId { get; set; }
        public List<Role> SelectedRoles { get; set; }
        public List<ApiUserInternal> SelectedUsers { get; set; }

    }
}
