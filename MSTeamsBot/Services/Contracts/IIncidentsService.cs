using MSTeamsBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSTeamsBot.Services.Contracts
{
    public interface IIncidentsService
    {
        Task<string> AddIncident(IncidentForm incident);

        Task<IList<Incident>> GetLatestIncidents(string count);

        Task<Incident> GetIncidentById(string id);
    }
}
