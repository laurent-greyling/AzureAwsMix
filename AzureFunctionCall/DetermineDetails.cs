using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace AzureFunctionCall
{
    public class DetermineDetails
    {
        public static Details Get(Message queueMessage)
        {
            var id = queueMessage.UserProperties.FirstOrDefault(x => x.Key == "Id").Value.ToString();
            var messageBody = Encoding.UTF8.GetString(queueMessage.Body);

            var details = JsonConvert.DeserializeObject<Details>(messageBody);
            details.FullName = $"{ details.FirstName } {details.Surname}";
            details.Id = id;

            return details;
        }
    }
}
