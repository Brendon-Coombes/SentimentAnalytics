using System;

namespace SentimentAnalytics.Common
{
    /// <summary>
    /// Class for storing keys for App Settings or local.settings.json if running locally
    /// </summary>
    public static class SettingsManager
    {
        private static class ConnectionStrings
        {
            public const string DefaultConnection = "DefaultConnection";
        }

        private static class Values
        {
            public const string AzureWebJobsStorageKey = "AzureWebJobsStorage";
            public const string TextAnalyticsUrlKey = "TextAnalyticsUrl";
            public const string TextAnalyticsKey = "TextAnalyticsKey";
            public const string SentimentTableNameKey = "SentimentTableName";

            public const string TwitterConsumer = "TwitterConsumerKey";
            public const string TwitterConsumerSecretKey = "TwitterConsumerSecret";
            public const string TwitterUserAccessTokenKey = "TwitterUserAccessToken";
            public const string TwitterUserAccessSecretKey = "TwitterUserAccessSecret";

            public const string RedditAppKey = "RedditAppId";
            public const string RedditAppSecretKey = "RedditAppSecret";
            public const string RedditUsernameKey = "RedditUsername";
            public const string RedditPasswordKey = "RedditPassword"; 
        }

        public static string GetConfigValue(string key)
        {
            return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }

        public static string AzureWebJobsStorage()
        {
            return GetConfigValue(Values.AzureWebJobsStorageKey);
        }

        public static string TextAnalyticsUrl()
        {
            return GetConfigValue(Values.TextAnalyticsUrlKey);
        }

        public static string TextAnalyticsApplicationKey()
        {
            return GetConfigValue(Values.TextAnalyticsKey);
        }

        public static string SentimentTableName()
        {
            return GetConfigValue(Values.SentimentTableNameKey);
        }

        public static string TwitterConsumerKey()
        {
            return GetConfigValue(Values.TwitterConsumer);
        }

        public static string TwitterConsumerSecret()
        {
            return GetConfigValue(Values.TwitterConsumerSecretKey);
        }

        public static string TwitterUserAccessToken()
        {
            return GetConfigValue(Values.TwitterUserAccessTokenKey);
        }

        public static string TwitterUserAccessSecret()
        {
            return GetConfigValue(Values.TwitterUserAccessSecretKey);
        }

        public static string RedditAppid()
        {
            return GetConfigValue(Values.RedditAppKey);
        }

        public static string RedditAppSecret()
        {
            return GetConfigValue(Values.RedditAppSecretKey);
        }

        public static string RedditUsername()
        {
            return GetConfigValue(Values.RedditUsernameKey);
        }

        public static string RedditPassword()
        {
            return GetConfigValue(Values.RedditPasswordKey);
        }
    }
}
