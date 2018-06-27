using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace MyIncidentsBot.Dialogs
{
    [LuisModel("8b1f49d2-67d0-4ce7-871c-87ce3bf46c38", "166d2616418f4c7cba18bac99764777a")]
    [Serializable]
    public class LUISDialog : LuisDialog<object>
    {
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("CreateIncident")]
        public async Task CreateIncident(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Let's create an incident.");
            context.Wait(MessageReceived);
        }
    }
}