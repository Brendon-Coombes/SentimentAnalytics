using Newtonsoft.Json;

namespace SentimentAnalytics.Models.TextAnalytics
{
    public class DocumentRequest
    {
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}