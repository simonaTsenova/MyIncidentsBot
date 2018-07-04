using System.Configuration;

namespace MyIncidentsBot.Helpers
{
    public static class Constants
    {
        public const string LUIS_MODEL_ID = "8b1f49d2-67d0-4ce7-871c-87ce3bf46c38";
        public const string LUIS_SUBSCRIPTION_KEY = "166d2616418f4c7cba18bac99764777a";
        public const string TOKEN_RESOURCE = "oauth_token.do";
        public const string INCIDENT_RESOURCE = "api/now/table/incident";
        public const  string LATEST_INCIDENTS_COUNT = "5";
    }
}