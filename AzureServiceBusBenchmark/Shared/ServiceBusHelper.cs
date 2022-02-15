using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace Shared
{
    public class ServiceBusHelper
    {
        const string NAMESPACE = "primary-canada-east.servicebus.windows.net";
        const string TOPIC_NAME = "AzureServiceBusBenchmark";
        private readonly ServiceBusClient _client;

        public ServiceBusHelper()
        {
            TokenCredential cred = new DefaultAzureCredential();
            _client = new ServiceBusClient(NAMESPACE, credential: cred);
        }

        public ServiceBusSender GetTopicSender() 
            => _client.CreateSender(TOPIC_NAME);
    }
}
