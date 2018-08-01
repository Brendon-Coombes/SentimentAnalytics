using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using RedditSharp;
using SentimentAnalytics.Common;
using SentimentAnalytics.Models;

namespace SentimentAnalytics
{
    public static class RetrieveRedditDataFunction
    {
        [FunctionName("RetrieveRedditDataFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            try
            {
                //TODO - Get real redirect URL: 
                BotWebAgent webAgent = new BotWebAgent(SettingsManager.RedditUsername(), SettingsManager.RedditPassword(), SettingsManager.RedditAppid(), SettingsManager.RedditAppSecret(), "http://www.example.com/unused/redirect/uri");

                SentimentService sentimentService = new SentimentService();
              
                               
                return new OkObjectResult("Posts saved successfully");
            }
            catch (Exception e)
            {
                log.Error("Uncaught exception occurred", e);
                return new BadRequestObjectResult("An unexpected error occurred processing your request");
            }
        }

        //TODO move to extension and merge with Data project version.
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }
    }
}
