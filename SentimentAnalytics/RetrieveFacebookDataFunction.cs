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
using SentimentAnalytics.Common;
using SentimentAnalytics.Models;

namespace SentimentAnalytics
{
    public static class RetrieveFacebookDataFunction
    {
        [FunctionName("RetrieveFacebookDataFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            try
            {
                //Read in request body
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                FacebookDataRequest data = JsonConvert.DeserializeObject<FacebookDataRequest>(requestBody);

                SentimentService sentimentService = new SentimentService();

                List<AnalyseTextRequest> requests = new List<AnalyseTextRequest>();
                foreach (var item in data.Posts.Where(x => x?.Data?.FirstOrDefault() != null))
                {
                    var sentimentEntity = await sentimentService.GetSentiment(new AnalyseTextRequest
                    {
                        ContentCreatedOn = UnixTimeStampToDateTime(double.Parse(item.Timestamp)),
                        Medium = "Facebook",
                        Text = item.Data.First().Post
                    });

                    var result = await sentimentService.SaveSentimentResult(sentimentEntity);
                    log.Info($"Sentiment Detected: {sentimentEntity.SentimentRating}");
                }
                               
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
