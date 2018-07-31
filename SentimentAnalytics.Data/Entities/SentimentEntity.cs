using System;
using Microsoft.WindowsAzure.Storage.Table;
using SentimentAnalytics.Data.Extensions;

namespace SentimentAnalytics.Data.Entities
{
    public class SentimentEntity : TableEntity
    {
        public SentimentEntity(string medium, DateTime dateAndTimePosted, string rawContent, double sentimentRating, string sentimentJson)
        {
            this.PartitionKey = medium;
            this.RowKey = dateAndTimePosted.ToUnixTime().ToString();
            DateAndTimePosted = dateAndTimePosted;
            RawContent = rawContent;
            SentimentRating = sentimentRating;
            SentimentJson = sentimentJson;
        }

        //Empty constructor is needed when retrieving data
        public SentimentEntity() { }

        public string Medium => this.PartitionKey;
        public DateTime DateAndTimePosted { get; set; }
        public string RawContent { get; set; }
        public double SentimentRating { get; set; }
        public string SentimentJson { get; set; }
    }
}
