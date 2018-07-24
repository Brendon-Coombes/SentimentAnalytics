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
    }
}
