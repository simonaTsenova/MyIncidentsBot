using MSTeamsBot.Models.Enums;
using System;

namespace MSTeamsBot.Models
{
    [Serializable]
    public class Incident
    {
        public UrgencyType? Urgency;
        public string Short_Description;
        public StateType State;
        public string Number;
        public string Sys_Created_On;
    }
}
