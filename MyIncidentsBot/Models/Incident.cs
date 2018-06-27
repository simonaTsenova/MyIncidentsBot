using Microsoft.Bot.Builder.FormFlow;
using MyIncidentsBot.Models.Enums;
using System;

namespace MyIncidentsBot.Models
{
    [Serializable]
    public class Incident
    {
        public UrgencyType? Urgency;
        public string Description;

        public static IForm<Incident> BuildForm()
        {
            return new FormBuilder<Incident>()
                    .Message("Let's create an incident!")
                    .Build();
        }
    }
}