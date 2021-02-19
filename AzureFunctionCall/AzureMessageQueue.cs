using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionCall
{
    public class AzureMessageQueue
    {
        private static string _queueConnection = Environment.GetEnvironmentVariable("AzureWebJobsServiceBus");
        private static string _queueName = "userdetails";

        public static async Task Add(IEnumerable<Details> details)
        {
            var userDetails = JsonConvert.SerializeObject(details);
            var queueClient = new QueueClient(_queueConnection, _queueName);

            var message = new Message(Encoding.UTF8.GetBytes(userDetails));
            message.UserProperties.Add("request", Guid.NewGuid());

            await queueClient.SendAsync(message);
        }
    }
}
