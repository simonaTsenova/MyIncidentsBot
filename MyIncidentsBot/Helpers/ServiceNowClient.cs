using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyIncidentsBot.Helpers
{
    public class TokenResponse
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
    }

    public class ServiceNowClient
    {
        private readonly string SERVICE_NOW_ENDPOINT;
        private readonly string GRANT_TYPE;
        private readonly string CLIENT_ID;
        private readonly string CLIENT_SECRET;
        private readonly string USERNAME;
        private readonly string PASSWORD;
        private readonly RestClient client;
        private readonly HttpClient httpClient;

        public ServiceNowClient()
        {
            this.SERVICE_NOW_ENDPOINT = ConfigurationManager.AppSettings["ServiceNowEndpoint"];
            this.GRANT_TYPE = ConfigurationManager.AppSettings["GrantType"];
            this.CLIENT_ID = ConfigurationManager.AppSettings["ClientId"];
            this.CLIENT_SECRET = ConfigurationManager.AppSettings["ClientSecret"];
            this.USERNAME = ConfigurationManager.AppSettings["Username"];
            this.PASSWORD = ConfigurationManager.AppSettings["Password"];

            this.client = new RestClient(SERVICE_NOW_ENDPOINT);
            this.httpClient = new HttpClient() { BaseAddress = new Uri(SERVICE_NOW_ENDPOINT) };
        }

        public async Task<HttpResponseMessage> GET(string resource, IDictionary<string, string> parameters)
        {
            var token = this.GetAccessToken();
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
            var token = this.GetAccessToken();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsJsonAsync(resource, body);

            return response;
        }

        private string GetAccessToken()
        {
            var request = new RestRequest(Constants.TOKEN_RESOURCE, Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", GRANT_TYPE);
            request.AddParameter("client_id", CLIENT_ID);
            request.AddParameter("client_secret", CLIENT_SECRET);
            request.AddParameter("username", USERNAME);
            request.AddParameter("password", PASSWORD);

            var response = this.client.Execute<TokenResponse>(request);
            var access_token = response.Data.Access_Token;

            return access_token;
        }
    }
}