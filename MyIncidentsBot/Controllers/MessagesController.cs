using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MyIncidentsBot.Dialogs;
using MyIncidentsBot.Services.Contracts;

namespace MyIncidentsBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly LUISDialog rootDialog;
        private readonly IMetaMessagingService metaMessagingService;

        public MessagesController(LUISDialog rootDialog, IMetaMessagingService metaMessagingService)
        {
            this.rootDialog = rootDialog;
            this.metaMessagingService = metaMessagingService;
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await this.metaMessagingService.SendTypingIndicator(activity);

                await Conversation.SendAsync(activity, () => this.rootDialog);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
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