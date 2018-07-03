using Microsoft.Bot.Builder.FormFlow;
using MyIncidentsBot.Models.Enums;
using System;

namespace MyIncidentsBot.Models
{
    [Serializable]
    public class Incident
    {
        public string ID;
        public UrgencyType? Urgency;
        public string Description;
        public string State;

        public static IForm<Incident> BuildForm()
        {
            return new FormBuilder<Incident>()
                    .Field(nameof(Urgency))
                    .Field(nameof(Description))
                    .Confirm("Do you want to create incident with DESCRIPTION: **{Description}** and URGENCY: **{Urgency}**?")
                    .Build();
        }
    }
}
