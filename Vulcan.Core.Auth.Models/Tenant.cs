using System;

namespace Vulcan.Core.Auth.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public bool IsStoreCreated { get; set; }
        public bool IsInitialConfigCompleted { get; set; }
        public string TempData { get; set; }
        public string StoreCreateJobKeyHash { get; set; }
        public string StoreCreateJobKeySalt { get; set; }
    }
}
