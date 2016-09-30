using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Vulcan.Core.TenantDatabaseGenerateJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static string _servicesBusConnectionString;
        private static NamespaceManager _namespaceManager;
        static void Main(string[] args)
        {
            _servicesBusConnectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.ServiceBus);
            _namespaceManager = NamespaceManager.CreateFromConnectionString(_servicesBusConnectionString);
            //CreateStartMessage();

            JobHostConfiguration config = new JobHostConfiguration();
            ServiceBusConfiguration serviceBusConfig = new ServiceBusConfiguration
            {
                ConnectionString = _servicesBusConnectionString
            };
            config.UseServiceBus(serviceBusConfig);

            JobHost host = new JobHost(config);
            host.RunAndBlock();
#if DEBUG
            Console.Read();
#endif
        }
        private static void CreateStartMessage()
        {
            if (!_namespaceManager.QueueExists(Functions.StartQueueName))
            {
                _namespaceManager.CreateQueue(Functions.StartQueueName);
            }

            QueueClient queueClient = QueueClient.CreateFromConnectionString(_servicesBusConnectionString, Functions.StartQueueName);

            using (Stream stream = new MemoryStream())
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.Write("Start");
                writer.Flush();
                stream.Position = 0;

                queueClient.Send(new BrokeredMessage(stream) { ContentType = "text/plain" });
            }

            queueClient.Close();
        }
    }
}
