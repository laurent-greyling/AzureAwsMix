using Microsoft.Azure.Cosmos.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;
using CosmosStorageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount;

namespace AzureFunctionCall
{
    public class AzureStorage
    {
        private static string _connection => Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public static async Task WriteBlob(Details details)        
        {
            var userDetails = JsonConvert.SerializeObject(details);
            var blob = await CreateBlobAsync(details.Id);

            await blob.UploadTextAsync(userDetails);
        }

        public static async Task WriteTable(Details details) 
        {
            var table = CreateTableAsync();
            
            var operation = TableOperation.Insert(details);

            await table.ExecuteAsync(operation);
        }

        public static async Task<string> ReadBlob(string blobName) 
        {
            var blob = await CreateBlobAsync(blobName);
            return await blob.DownloadTextAsync();
        }

        public static async Task<List<Details>> ReadTable(string partitionKey)
        {
            var table = CreateTableAsync();

            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<Details>().Where(condition);

            var items = new List<Details>();
            TableContinuationToken token = null;

            do
            {
                var seg = await table.ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);

            } while (token != null);

            return items;
        }

        private static async Task<CloudBlockBlob> CreateBlobAsync(string blobName)
        {
            var account = CloudStorageAccount.Parse(_connection);
            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("userdetails");
            await container.CreateIfNotExistsAsync();

            return container.GetBlockBlobReference(blobName);
        }

        private static CloudTable CreateTableAsync()
        {
            var account = CosmosStorageAccount.Parse(_connection);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference("userdetails");

            table.CreateIfNotExists();

            return table;
        }
    }
}
