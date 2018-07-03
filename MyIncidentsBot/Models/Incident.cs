using MyIncidentsBot.Models.Enums;
using System;

namespace MyIncidentsBot.Models
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