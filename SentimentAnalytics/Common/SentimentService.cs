using SentimentAnalytics.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using SentimentAnalytics.Data.Entities;
using SentimentAnalytics.Data.Services;
using SentimentAnalytics.Models;
using SentimentAnalytics.Models.TextAnalytics;

namespace SentimentAnalytics.Common
{
    public class SentimentService
    {
        public async Task<SentimentEntity> GetSentiment(AnalyseTextRequest request)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(SettingsManager.TextAnalyticsUrl());
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SettingsManager.TextAnalyticsApplicationKey());

            var result = await client.PostAsJsonAsync("sentiment",
                new SentimentRequest
                {
                    Documents = new List<DocumentRequest>
                    {
                        new DocumentRequest
                        {
                            Id = "1",
                            Language = "en",
                            Text = request.Text
                        }
                    }
                }
            );

            string rawResult = await result.Content.ReadAsStringAsync();
            var sentimentResult = JObject.Parse(rawResult).ToObject<SentimentResult>().Documents.First();

            return new SentimentEntity(request.Medium, request.ContentCreatedOn, request.Text, sentimentResult.Score, rawResult);
        }

        public async Task<TableResult> SaveSentimentResult(SentimentEntity entity)
        {
            IAzureTableStorageService tableStorageService = new AzureTableStorageService(SettingsManager.AzureWebJobsStorage(), SettingsManager.SentimentTableName());
            return await tableStorageService.InsertEntity(entity);
        }

        public async Task<TableResult> RetrieveEntity(string partitionKey, string rowKey)
        {
            IAzureTableStorageService tableStorageService = new AzureTableStorageService(SettingsManager.AzureWebJobsStorage(), SettingsManager.SentimentTableName());
            return await tableStorageService.GetEntity(partitionKey, rowKey);
        }
    }
}
