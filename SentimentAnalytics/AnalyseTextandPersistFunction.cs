using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using SentimentAnalytics.Common;
using SentimentAnalytics.Data.Entities;
using SentimentAnalytics.Data.Interfaces;
using SentimentAnalytics.Data.Services;
using SentimentAnalytics.Models;
using SentimentAnalytics.Models.TextAnalytics;

namespace SentimentAnalytics
{
    public static class AnalyseTextandPersistFunction
    {
        [FunctionName("AnalyseTextandPersistFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            try
            {            
                //Read in request body
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                AnalyseTextRequest data = JsonConvert.DeserializeObject<AnalyseTextRequest>(requestBody);

                //Validate request was passed
                if (data == null)
                {
                    return new BadRequestObjectResult("Please pass a valid request body");
                }

                //Log the data that was posted to us out
                log.Info($"Data - Medium: {data.Medium}{Environment.NewLine} Data - Posted On {data.ContentCreatedOn}{Environment.NewLine} Data - Text {data.Text}");

                //Add basic validation - we can't process anything without all of the fields
                if (string.IsNullOrEmpty(data.Medium))
                {
                    return new BadRequestObjectResult("Please include a medium in the request body");
                }

                if (string.IsNullOrEmpty(data.Text) || data.Text.Length > 5000)
                {
                    return new BadRequestObjectResult("Please include text that is less than 5000 characters");
                }

                if (data.ContentCreatedOn == default(DateTime))
                {
                    return new BadRequestObjectResult("Please include a valid date for the content created on time and date");
                }
                
                //Get Sentiment and save to table
                var entity = await GetSentiment(data);
                var result = await SaveSentimentResult(entity);
                
                return new OkObjectResult($"Sentiment Detected: {entity?.SentimentRating} - Saved to table status code: {result.HttpStatusCode}");
            }
            catch (Exception e)
            {
                log.Error("Uncaught exception occurred", e);
                return new BadRequestObjectResult("An unexpected error occurred processing your request");
            }
        }

        private static async Task<SentimentEntity> GetSentiment(AnalyseTextRequest request)
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

        private static async Task<TableResult> SaveSentimentResult(SentimentEntity entity)
        {
            IAzureTableStorageService tableStorageService = new AzureTableStorageService(SettingsManager.AzureWebJobsStorage(), SettingsManager.SentimentTableName());
            return await tableStorageService.InsertEntity(entity);
        }

        //TODO: Move into new function/app
        private static async Task<TableResult> RetrieveEntity(string partitionKey, string rowKey)
        {
            IAzureTableStorageService tableStorageService = new AzureTableStorageService(SettingsManager.AzureWebJobsStorage(), SettingsManager.SentimentTableName());
            return await tableStorageService.GetEntity(partitionKey, rowKey);
        }
    }
}
