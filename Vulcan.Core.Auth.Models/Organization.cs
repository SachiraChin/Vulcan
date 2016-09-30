using System;

namespace Vulcan.Core.Auth.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Timezone { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
