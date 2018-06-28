using MyIncidentsBot.Models.Enums;

namespace MyIncidentsBot.Models
{
    public class Incident
    {
        public string ID;
        public UrgencyType? Urgency;
        public string Description;
        public string State;
    }
}