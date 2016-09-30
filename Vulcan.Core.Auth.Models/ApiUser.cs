using System.ComponentModel.DataAnnotations;

namespace Vulcan.Core.Auth.Models
{
    public class ApiUser
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Range(6, int.MaxValue)]
        public int? TokenExpireTimeMinutes { get; set; }
        public bool IsSystem { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
    }
}
