using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using SentimentAnalytics.Data.Entities;

namespace SentimentAnalytics.Data.Interfaces
{
    public interface IAzureTableStorageService
    {
        Task<TableResult> InsertEntity(TableEntity entity);
        Task<IList<TableResult>> InsertBatch(IList<TableEntity> batchData);
        Task<TableResult> GetEntity(string partitionKey, string rowKey);
    }
}
