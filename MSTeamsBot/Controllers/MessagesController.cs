using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MSTeamsBot.Dialogs;
using MSTeamsBot.Services.Contracts;

namespace MSTeamsBot.Controllers
{
    [Route("api/[controller]")]
    //[BotAuthentication]
    public class MessagesController : Controller
    {
        private readonly LUISDialog rootDialog;
        private readonly IMetaMessagingService metaMessagingService;

        public MessagesController(LUISDialog rootDialog, IMetaMessagingService metaMessagingService)
        {
            this.rootDialog = rootDialog;
            this.metaMessagingService = metaMessagingService;
        }

        [HttpPost]
        public async Task Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                //await this.metaMessagingService.SendTypingIndicator(activity);

                await Conversation.SendAsync(activity, () => this.rootDialog);
            }
            else
            {
                await HandleSystemMessage(activity);
            }
        }

        private async Task<object> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
