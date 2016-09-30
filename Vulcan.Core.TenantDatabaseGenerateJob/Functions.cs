using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Vulcan.Core.Auth.DataContexts;

namespace Vulcan.Core.TenantDatabaseGenerateJob
{
    public class Functions
    {
        public const string StartQueueName = "Vulcancoretenantdatabasegeneratequeue";

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger(StartQueueName)] string message, TextWriter log)
        {
            WriteLog(log, $"Queue message recived for tenant id: {message}");
            CreateTenantDatabase(log, message);
        }

        [NoAutomaticTrigger]
        public static async void CreateTenantDatabase(TextWriter log, string tenantId)
        {
            using (var internalDataContext = new InternalDataContext())
            {
                var tid = new Guid(tenantId);
                try
                {
                    await internalDataContext.CreateTenantDatastore(tenantId, logEntry =>
                    {
                        internalDataContext.TenantDeploymentLogsDataSet.AddLogEntry(tid, logEntry);
                        WriteLog(log, logEntry);
                    });

                    await internalDataContext.Tenants.SetStoreCreated(tid);
                }
                catch (Exception ex)
                {
                    var le = $"Job failed: {ex.Message}\n{ex.StackTrace}";

                    internalDataContext.TenantDeploymentLogsDataSet.AddLogEntry(tid, le);
                    WriteLog(log, le);
                }
            }
        }

        public static void WriteLog(TextWriter log, string logEntry)
        {
            Console.WriteLine(logEntry);
            log.WriteLine(logEntry);
        }
    }
}
