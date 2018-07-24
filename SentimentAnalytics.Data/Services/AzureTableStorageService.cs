using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using SentimentAnalytics.Data.Interfaces;
using Microsoft.WindowsAzure.Storage;


namespace SentimentAnalytics.Data.Services
{
    public class AzureTableStorageService : IAzureTableStorageService
    {
        private readonly CloudTable _cloudTable;

        public AzureTableStorageService(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            _cloudTable = tableClient.GetTableReference(tableName);
            _cloudTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<TableResult> InsertEntity(TableEntity entity)
        {
            TableOperation operation = TableOperation.Insert(entity);
            return await _cloudTable.ExecuteAsync(operation);
        }

        public async Task<IList<TableResult>> InsertBatch(IList<TableEntity> batchData)
        {
            if (batchData.Count > 100)
            {
                throw new InvalidOperationException("Azure Tables can only handle batches of up to 100");
            }

            TableBatchOperation operation = new TableBatchOperation();

            foreach (var dataItem in batchData)
            {
                operation.Insert(dataItem);
            }

            return await _cloudTable.ExecuteBatchAsync(operation);
        }

        public async Task<TableResult> GetEntity(string partitionKey, string rowKey)
        {
            TableOperation operation = TableOperation.Retrieve<TableEntity>(partitionKey, rowKey);
            return await _cloudTable.ExecuteAsync(operation);
        }
    }
}
