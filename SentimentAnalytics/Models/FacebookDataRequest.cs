using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SentimentAnalytics.Models
{
    public class FacebookDataRequest
    {
        [JsonProperty("status_updates")]
        public List<FacebookPost> Posts { get; set; }
    }

    public class FacebookPost
    {
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("data")]
        public List<FacebookPostData> Data { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class FacebookPostData
    {
        [JsonProperty("post")]
        public string Post { get; set; }
    }
}
