using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace MSTeamsBot.Services.Contracts
{
    public interface IMetaMessagingService
    {
        Task SendTypingIndicator(Activity activity);
    }
}
