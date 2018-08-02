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
using RedditSharp.Things;
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

                var reddit = new Reddit(webAgent);
                var user = await reddit.GetUserAsync(SettingsManager.RedditUsername());
                var comments = user.GetComments(-1);

                IList<AnalyseTextRequest> analyseRequests = new List<AnalyseTextRequest>();
                await comments.ForEachAsync(c =>
                {
                    analyseRequests.Add(new AnalyseTextRequest
                    {
                        ContentCreatedOn = c.Created,
                        Medium = "Reddit",
                        Text = c.Body
                    });                   
                });

                SentimentService sentimentService = new SentimentService();

                foreach (var comment in analyseRequests)
                {
                    try
                    {
                        log.Info($"Getting sentiment and saving comment: {comment.Text}");
                        var result = await sentimentService.GetSentiment(comment);
                        await sentimentService.SaveSentimentResult(result);
                    }
                    catch (Exception e)
                    {
                        log.Error($"{e.Message}");
                        log.Error($"Error saving comment {comment.Text}");
                    }
                }
                               
                return new OkObjectResult("Posts saved successfully");
            }
            catch (Exception e)
            {
                log.Error("Uncaught exception occurred", e);
                return new BadRequestObjectResult("An unexpected error occurred processing your request");
            }
        }
    }
}
