using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using MyIncidentsBot.Models;

namespace MyIncidentsBot.Dialogs
{
    [Serializable]
    public class RootDialog
    {
        public static readonly IDialog<string> dialog = Chain.PostToChain().Select(message => message.Text)
            .Switch(
                new RegexCase<IDialog<string>>(
                    new Regex("^create", RegexOptions.IgnoreCase),
                    (context, text) => (IDialog<string>)FormDialog.FromForm(Incident.BuildForm, FormOptions.PromptInStart)
                    .ContinueWith(AfterCreateIncident)),
                new RegexCase<IDialog<string>>(
                    new Regex("^all", RegexOptions.IgnoreCase),
                    (context, text) => new GetMyIncidentsDialog().ContinueWith(AfterGetMyIncidents))
                //new RegexCase<IDialog<string>>(
                //    new Regex(),
                //    (context, text) => )
            ).Unwrap().PostToUser();

        private static async Task<IDialog<string>> AfterCreateIncident(IBotContext context, IAwaitable<Incident> result)
        {
            var message = await result;

            return Chain.Return("Successfully created incident.");
        }

        private static async Task<IDialog<string>> AfterGetMyIncidents(IBotContext context, IAwaitable<object> result)
        {
            var message = await result;

            return Chain.Return("Successfully got your incidents.");
        }
    }
}