using System.Collections.Generic;
using Newtonsoft.Json;

namespace SentimentAnalytics.Models.TextAnalytics
{
    public class SentimentResult
    {
        [JsonProperty("documents")]
        public IList<DocumentResult> Documents { get; set; }
    }
}