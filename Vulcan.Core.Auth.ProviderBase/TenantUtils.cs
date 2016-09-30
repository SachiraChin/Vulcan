using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Vulcan.Core.Auth.DataContexts;
using Vulcan.Core.Auth.Providers;

namespace Vulcan.Core.Auth
{
    public class TenantUtils
    {

        public struct TenantStatus
        {
            public string Id { get; set; }
            public bool IsCreating { get; set; }
            public string JobKey { get; set; }
        }

        public static TenantStatus GetInternalTenant(string externalTenantId, string tempData)
        {
            var internalTenant = new TenantStatus();

            using (var internalDataContext = new InternalDataContext())
            {
                internalTenant.Id = internalDataContext.ExternalTenants.GetInternalByExternalTenantId(externalTenantId);

                if (internalTenant.Id != null) return internalTenant;

                var jobKey = Guid.NewGuid().ToString("N");
                var hash = PasswordHashProvider.CreateHash(jobKey);

                internalTenant.Id = internalDataContext.ExternalTenants.AddExternalTenant(externalTenantId);
                internalTenant.IsCreating = true;
                internalTenant.JobKey = jobKey;

                var tid = new Guid(internalTenant.Id);
                internalDataContext.Tenants.SetStoreCreateKey(tid, hash.Hash, hash.Salt);
                internalDataContext.Tenants.SetTempData(tid, tempData);

                var client = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString, "Vulcancoretenantdatabasegeneratequeue");
                
                client.Send(new BrokeredMessage(internalTenant.Id));
            }

            return internalTenant;
        }
    }
}
