using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace MyIncidentsBot.Services.Contracts
{
    public interface  IMetaMessagingService
    {
        Task SendTypingIndicator(Activity activity);
    }
}