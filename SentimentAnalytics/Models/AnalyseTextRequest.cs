using System;

namespace SentimentAnalytics.Models
{
    public class AnalyseTextRequest
    {
        public string Medium { get; set; }
        public DateTime ContentCreatedOn { get; set; }
        public string Text { get; set; }
    }
}
