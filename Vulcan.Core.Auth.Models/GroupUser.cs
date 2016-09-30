using System;

namespace Vulcan.Core.Auth.Models
{
    public class GroupUser
    {
        public long GroupId { get; set; }
        public long ApiUserId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
