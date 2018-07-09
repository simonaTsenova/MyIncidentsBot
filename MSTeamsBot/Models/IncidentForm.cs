using Microsoft.Bot.Builder.FormFlow;
using MSTeamsBot.Models.Enums;
using System;

namespace MSTeamsBot.Models
{
    [Serializable]
    public class IncidentForm
    {
        public UrgencyType? Urgency;
        public string Description;

        public static IForm<IncidentForm> BuildForm()
        {
            return new FormBuilder<IncidentForm>()
                    .Field(nameof(Urgency))
                    .Field(nameof(Description))
                    .Confirm("Do you want to create incident with DESCRIPTION: **{Description}** and URGENCY: **{Urgency}**?")
                    .Build();
        }
    }
}
