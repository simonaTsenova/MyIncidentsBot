using Microsoft.Bot.Builder.FormFlow;
using MyIncidentsBot.Helpers;
using MyIncidentsBot.Models.Enums;
using System;

namespace MyIncidentsBot.Models
{
    [Serializable]
    public class IncidentForm
    {
        public UrgencyType? Urgency;
        public string Description;

        public static IForm<IncidentForm> BuildForm()
        {
            return FormCustomizer.CreateCustomForm<IncidentForm>()
                    .Field(nameof(Urgency))
                    .Field(nameof(Description), "Please, describe you problem in few words (shortly).")
                    .Confirm("Are you sure you want to create incident with DESCRIPTION: **{Description}** and URGENCY: **{Urgency}**?")
                    .Build();
        }

        
    }
}