using MSTeamsBot.Helpers.Contracts;
using MSTeamsBot.Models;
using MSTeamsBot.Models.Responses;
using MSTeamsBot.Services.Contracts;
using MSTeamsBot.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSTeamsBot.Services
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
            var parameters = new Dictionary<string, string>
            {
                { "sysparm_query", "ORDERBYDESCsys_created_on" },
                { "sysparm_limit", count },
                { "sysparm_fields", "short_description,number,state,urgency,sys_created_on" }
            };

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
            var parameters = new Dictionary<string, string>
            {
                { "number", id },
                { "sysparm_fields", "short_description,number,state,urgency,sys_created_on" }
            };

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
    }
}
