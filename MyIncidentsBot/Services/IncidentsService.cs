using MyIncidentsBot.Helpers;
using MyIncidentsBot.Helpers.Contracts;
using MyIncidentsBot.Models;
using MyIncidentsBot.Services.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyIncidentsBot.Services
{
    public class IncidentResponse
    {
        public Incident Result { get; set; }
    }

    public class IncidentsCollectionResponse
    {
        public List<Incident> Result { get; set; }
    }

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
                caller_id = "4e4be61bdb491300afbff3de3b961903",
                urgency = ((int)incident.Urgency).ToString()
            };

            var response = await this.client.POST(Constants.INCIDENT_RESOURCE, incidentObject);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                var deserializedIncident = JsonConvert.DeserializeObject<IncidentResponse>(responseString);

                return deserializedIncident.Result.Number;
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IList<Incident>> GetLatestIncidents()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sysparm_query", "ORDERBYDESCsys_created_on");
            parameters.Add("sysparm_limit", Constants.LATEST_INCIDENTS_COUNT);
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
    }
}