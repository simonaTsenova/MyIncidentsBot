using Microsoft.Bot.Builder.FormFlow;
using MyIncidentsBot.Models.Enums;
using System;

namespace MyIncidentsBot.Models
{
    [Serializable]
    public class Incident
    {
        [Optional]
        public string ID;
        public UrgencyType? Urgency;
        public string Description;
        [Optional]
        public string State;

        public static IForm<Incident> BuildForm()
        {
            return new FormBuilder<Incident>()
                    .Build();
        }
    }
}