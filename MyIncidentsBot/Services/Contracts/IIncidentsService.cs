using Microsoft.Bot.Builder.Dialogs;
using MyIncidentsBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyIncidentsBot.Services.Contracts
{
    public interface IIncidentsService
    {
        Task<string> AddIncident(IncidentForm incident, string callerEmail);

        Task<IList<Incident>> GetLatestIncidents(string count);

        Task<Incident> GetIncidentById(string id);
    }
}