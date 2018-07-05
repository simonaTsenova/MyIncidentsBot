using Microsoft.Bot.Connector;
using MyIncidentsBot.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace MyIncidentsBot.Services
{
    [Serializable]
    public class MetaMessagingService : IMetaMessagingService
    {
        public async Task SendTypingIndicator(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var isTypingReply = activity.CreateReply("Typing ...");
            isTypingReply.Type = ActivityTypes.Typing;
            await connector.Conversations.ReplyToActivityAsync(isTypingReply);
        }
    }
}