using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MyIncidentsBot.Common;
using MyIncidentsBot.Helpers.Contracts;
using MyIncidentsBot.Models;
using MyIncidentsBot.Models.Responses;
using MyIncidentsBot.Services.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyIncidentsBot.Services
{
    [Serializable]
    public class IncidentsService : IIncidentsService
    {
        private readonly IRestClient client;

        public IncidentsService(IRestClient client)
        {
            this.client = client;
        }

        public async Task<string> AddIncident(IncidentForm incident)
        {
            var incidentObject = new
            {
                short_description = incident.Description,
                urgency = ((int)incident.Urgency).ToString()
            };

            var response = await this.client.POST(Constants.INCIDENT_RESOURCE, incidentObject);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var deserializedIncident = JsonConvert.DeserializeObject<IncidentResponse>(responseString);

                return deserializedIncident.Result.Number;
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IList<Incident>> GetLatestIncidents(string count)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sysparm_query", "ORDERBYDESCsys_created_on");
            parameters.Add("sysparm_limit", count);
            parameters.Add("sysparm_fields", "short_description,number,state,urgency,sys_created_on");

            var response = await this.client.GET(Constants.INCIDENT_RESOURCE, parameters);
            var responseString = await response.Content.ReadAsStringAsync();

            List<Incident> result = new List<Incident>();
            if (response.IsSuccessStatusCode)
            {
                var deserializedIncident = JsonConvert.DeserializeObject<IncidentsCollectionResponse>(responseString);
                result = deserializedIncident.Result;
            }

            return result;
        }

        public async Task<Incident> GetIncidentById(string id)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("number", id);
            parameters.Add("sysparm_fields", "short_description,number,state,urgency,sys_created_on");

            var response = await this.client.GET(Constants.INCIDENT_RESOURCE, parameters);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var deserializedIncident = JsonConvert.DeserializeObject<IncidentsCollectionResponse>(responseString);

                if (deserializedIncident.Result.Count == 1)
                {
                    return deserializedIncident.Result[0];
                }
            }

            return null;
        }

        public async Task SendTypingIndicator(IDialogContext context)
        {
            var activity = context.Activity as Activity;
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            Activity isTypingReply = activity.CreateReply("Typing ...");
            isTypingReply.Type = ActivityTypes.Typing;
            await connector.Conversations.ReplyToActivityAsync(isTypingReply);
        }
    }
}