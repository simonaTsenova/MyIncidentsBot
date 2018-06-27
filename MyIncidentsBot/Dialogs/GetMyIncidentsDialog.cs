using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace MyIncidentsBot.Dialogs
{
    [Serializable]
    public class GetMyIncidentsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Here's your incidents.");
            await context.PostAsync("incident 1");
            await context.PostAsync("incident 2");

            context.Done(true);
        }
    }
}