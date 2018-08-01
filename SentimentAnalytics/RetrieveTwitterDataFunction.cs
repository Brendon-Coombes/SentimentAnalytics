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
using System.Linq;
using SentimentAnalytics.Common;
using SentimentAnalytics.Models;
using Tweetinvi;

namespace SentimentAnalytics
{
    public static class RetrieveTwitterDataFunction
    {
        [FunctionName("RetrieveTwitterDataFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            try
            {
                Auth.SetUserCredentials(SettingsManager.TwitterConsumerKey(), SettingsManager.TwitterConsumerSecret(), SettingsManager.TwitterUserAccessToken(), SettingsManager.TwitterUserAccessSecret());
                var user = User.GetAuthenticatedUser();

                var tweets = Timeline.GetUserTimeline(user, 200);
                var nonRetweetedTweets = tweets.Where(x => x.RetweetedTweet == null).ToList();

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:7071/api/");


                foreach (var tweet in nonRetweetedTweets)
                {
                    var queueResult = await client.PostAsJsonAsync("AnalyseTextandPersistFunction",
                        new AnalyseTextRequest
                        {
                            Text = tweet.Text,
                            ContentCreatedOn = tweet.CreatedAt,
                            Medium = "Twitter"
                        }
                    );
                }

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
                
                SentimentService sentimentService = new SentimentService();

                //Get Sentiment and save to table
                var entity = await sentimentService.GetSentiment(data);
                var result = await sentimentService.SaveSentimentResult(entity);
                
                return new OkObjectResult($"Sentiment Detected: {entity?.SentimentRating} - Saved to table status code: {result.HttpStatusCode}");
            }
            catch (Exception e)
            {
                log.Error("Uncaught exception occurred", e);
                return new BadRequestObjectResult("An unexpected error occurred processing your request");
            }
        }
    }
}
