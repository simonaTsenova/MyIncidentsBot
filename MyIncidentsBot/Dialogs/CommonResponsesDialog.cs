using BestMatchDialog;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace MyIncidentsBot.Dialogs
{
    [Serializable]
    public class CommonResponsesDialog : BestMatchDialog<bool>
    {
        [BestMatch(new string[] { "hello", "hi", "hey there", "hello there", "hey", "hi there", "hello, man",
            "hi, man", "good morning", "good afternoon", "good evening" })]
        public async Task HandleGreeting(IDialogContext context, string messageText)
        {
            await context.PostAsync("Hey, there. How can I help you?");
            context.Done(true);
        }

        [BestMatch(new string[] { "bye", "bye bye", "see you later", "goodbye", "laters", "gotta go", "laters" })]
        public async Task HandleGoodbye(IDialogContext context, string messageText)
        {
            await context.PostAsync("Bye bye. See you soon.");
            context.Done(true);
        }

        [BestMatch(new string[] { "thank you", "thanks", "thank you very much" })]
        public async Task HandleGratefulness(IDialogContext context, string messageText)
        {
            await context.PostAsync("No problem. I'm always glad to help.");
            context.Done(true);
        }

        [BestMatch(new string[] { "how is it going", "how are you", "what's up", "how do you do", "how are you doing" })]
        public async Task HandleStatus(IDialogContext context, string messageText)
        {
            await context.PostAsync("I'm good, thanks for asking.");
            context.Done(true);
        }

        [BestMatch(new string[] { "okay", "ok", "i see", "good", "fine" })]
        public async Task HandleNoResponseMessage(IDialogContext context, string messageText)
        {
            context.Done(true);
        }

        public override async Task NoMatchHandler(IDialogContext context, string messageText)
        {
            context.Done(false);
        }
    }
}