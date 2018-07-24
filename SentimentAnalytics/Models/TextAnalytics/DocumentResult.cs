using Newtonsoft.Json;

namespace SentimentAnalytics.Models.TextAnalytics
{
    public class DocumentResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}