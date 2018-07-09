using System;

namespace MSTeamsBot.Common.Settings
{
    [Serializable]
    public class AppSettings
    {
        //public string MicrosoftAppId { get; set; }
        //public string MicrosoftAppPassword { get; set; }
        public string LuisModelId { get; set; }
        public string LuisSubscriptionKey { get; set; }
        public string ServiceNowEndpoint { get; set; }
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
