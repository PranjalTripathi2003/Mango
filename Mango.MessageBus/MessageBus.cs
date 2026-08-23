
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {

        // Ideally will be place in appSettings.json, but since its just a single MessageBus service, we can paste it here.
        private string connectionString = "Endpoint=sb://ptmangoweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=i2zvKkFZ0/hYPUwifZIroy3qBL2cNhWkv+ASbFEr6DQ=";
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);


            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);

            await client.DisposeAsync();


        }
    }
}
