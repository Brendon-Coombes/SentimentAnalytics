using System.Collections.Generic;
using Newtonsoft.Json;

namespace SentimentAnalytics.Models.TextAnalytics
{
    public class SentimentRequest
    {
        [JsonProperty("documents")]
        public IList<DocumentRequest> Documents { get; set; }
    }
}