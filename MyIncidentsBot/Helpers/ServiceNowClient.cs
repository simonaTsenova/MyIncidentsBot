using MyIncidentsBot.Common;
using MyIncidentsBot.Common.Exceptions;
using MyIncidentsBot.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyIncidentsBot.Helpers
{
    [Serializable]
    public class ServiceNowClient : Contracts.IRestClient
    {
        private readonly string SERVICE_NOW_ENDPOINT;
        private readonly string GRANT_TYPE;
        private readonly string CLIENT_ID;
        private readonly string CLIENT_SECRET;
        private readonly string USERNAME;
        private readonly string PASSWORD;

        public ServiceNowClient()
        {
            this.SERVICE_NOW_ENDPOINT = ConfigurationManager.AppSettings["ServiceNowEndpoint"];
            this.GRANT_TYPE = ConfigurationManager.AppSettings["GrantType"];
            this.CLIENT_ID = ConfigurationManager.AppSettings["ClientId"];
            this.CLIENT_SECRET = ConfigurationManager.AppSettings["ClientSecret"];
            this.USERNAME = ConfigurationManager.AppSettings["Username"];
            this.PASSWORD = ConfigurationManager.AppSettings["Password"];
        }

        public async Task<HttpResponseMessage> GET(string resource, IDictionary<string, string> parameters)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(SERVICE_NOW_ENDPOINT) };
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
            var httpClient = new HttpClient() { BaseAddress = new Uri(SERVICE_NOW_ENDPOINT) };
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
            var httpClient = new HttpClient() { BaseAddress = new Uri(SERVICE_NOW_ENDPOINT) };
            httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", GRANT_TYPE);
            parameters.Add("client_id", CLIENT_ID);
            parameters.Add("client_secret", CLIENT_SECRET);
            parameters.Add("username", USERNAME);
            parameters.Add("password", PASSWORD);

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