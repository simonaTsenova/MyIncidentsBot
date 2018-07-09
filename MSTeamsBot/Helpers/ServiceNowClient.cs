using Microsoft.Extensions.Options;
using MSTeamsBot.Common;
using MSTeamsBot.Common.Exceptions;
using MSTeamsBot.Common.Settings;
using MSTeamsBot.Helpers.Contracts;
using MSTeamsBot.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MSTeamsBot.Helpers
{
    [Serializable]
    public class ServiceNowClient : IRestClient
    {
        private readonly AppSettings appSettings;

        public ServiceNowClient(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public async Task<HttpResponseMessage> GET(string resource, IDictionary<string, string> parameters)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(appSettings.ServiceNowEndpoint) };
            var token = await this.GetAccessToken();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var withParams = resource + "?";
            foreach (var param in parameters)
            {
                withParams += param.Key + "=" + param.Value + "&";
            }

            var response = await httpClient.GetAsync(withParams);

            return response;
        }

        public async Task<HttpResponseMessage> POST(string resource, object body)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(appSettings.ServiceNowEndpoint) };
            var token = await this.GetAccessToken();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsJsonAsync(resource, body);

            return response;
        }

        private async Task<string> GetAccessToken()
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(appSettings.ServiceNowEndpoint) };
            httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", appSettings.GrantType },
                { "client_id", appSettings.ClientId },
                { "client_secret", appSettings.ClientSecret },
                { "username", appSettings.Username },
                { "password", appSettings.Password }
            };

            var response = await httpClient.PostAsync(Constants.TOKEN_RESOURCE, new FormUrlEncodedContent(parameters));
            var responseString = await response.Content.ReadAsStringAsync();
            var access_token = JsonConvert.DeserializeObject<TokenResponse>(responseString).Access_Token;

            if (access_token == null)
            {
                throw new AuthenticationFailedException();
            }

            return access_token;
        }
    }
}
