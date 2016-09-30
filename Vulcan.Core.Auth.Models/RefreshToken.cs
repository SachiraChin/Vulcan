using System;
using Vulcan.Core.DataAccess.Entities;

namespace Vulcan.Core.Auth.Models
{
    public class RefreshToken : DynamicEntity
    {
        public string TokenHash { get; set; }
        public string Subject { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Ticket { get; set; }
    }
}