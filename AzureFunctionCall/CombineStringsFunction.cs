using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctionCall
{
    public static class CombineStringsFunction
    {
        [FunctionName("CombineStringsFunction")]
        public static async Task Run([ServiceBusTrigger("namesurnamequeue")]Message myQueueItem, ILogger log)
        {
            try
            {
                var details = DetermineDetails.Get(myQueueItem);

                ////Write Read to Blob
                //await AzureStorage.WriteBlob(details);
                //var blobDetails = await AzureStorage.ReadBlob(details.Id);

                ////Write Read to TableStorage
                //details.PartitionKey = details.FirstName;
                //details.RowKey = $"{details.Surname}-{details.Id}";
                //await AzureStorage.WriteTable(details);
                //var tableDetails = await AzureStorage.ReadTable(details.FirstName);

                ////Write read to sql
                //AzureSqlStorage.Save(details);

                //await Task.Delay(5000);

                var sqlDetails = AzureSqlStorage.Read<Details>(x => x.FirstName == details.FirstName); // && x.Surname == details.Surname && x.Id == details.Id && x.FullName == details.FullName);

                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");                
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
