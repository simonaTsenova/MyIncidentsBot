using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyIncidentsBot.Helpers.Contracts
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> GET(string resource, IDictionary<string, string> parameters);

        Task<HttpResponseMessage> POST(string resource, object body);
    }
}