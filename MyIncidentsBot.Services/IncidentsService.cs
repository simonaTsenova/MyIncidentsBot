using MyIncidentsBot.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MyIncidentsBot.Services
{
    public class IncidentsService
    {
        private HttpClient httpClient;

        public void AddIncident(Incident incident)
        {

        }

        public IList<Incident> GetLatestIncidents()
        {
            throw new NotImplementedException();
        }

        public Incident GetIncidentById(string id)
        {
            throw new NotFiniteNumberException();
        }
    }
}
