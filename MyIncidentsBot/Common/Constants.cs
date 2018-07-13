using System.Configuration;

namespace MyIncidentsBot.Common
{
    public static class Constants
    {
        public const string LUIS_MODEL_ID = "8b1f49d2-67d0-4ce7-871c-87ce3bf46c38";
        public const string LUIS_SUBSCRIPTION_KEY = "166d2616418f4c7cba18bac99764777a";
        public const string TOKEN_RESOURCE = "oauth_token.do";
        public const string INCIDENT_RESOURCE = "api/now/table/incident";
        public const  string LATEST_INCIDENTS_COUNT = "20";

        public const string NONE_INTENT = "None";
        public const string CREATE_INCIDENT_INTENT = "CreateIncident";
        public const string GET_ALL_INCIDENTS_INTENT = "GetAllIncidents";
        public const string GET_LATEST_INCIDENTS_INTENT = "GetLatestIncidents";
        public const string GET_INCIDENT_STATE_INTENT = "GetIncidentState";
        public const string HELP_INTENT = "Help";
    }
}