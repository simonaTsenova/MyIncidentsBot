using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using MyIncidentsBot.Models;
using MyIncidentsBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyIncidentsBot.Dialogs
{
    [LuisModel("8b1f49d2-67d0-4ce7-871c-87ce3bf46c38", "166d2616418f4c7cba18bac99764777a")]
    [Serializable]
    public class LUISDialog : LuisDialog<object>
    {
        private readonly IList<Incident> incidents = new List<Incident>()
        {
            new Incident() { ID = "INC0019503", Urgency = UrgencyType.Medium, Description = "Node is down", State = "In progress" },
            new Incident() { ID = "INC0019504", Urgency = UrgencyType.Low, Description = "Button disabled", State = "Closed" }
        };

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("CreateIncident")]
        public async Task CreateIncident(IDialogContext context, LuisResult result)
        {
            try
            {
                await context.PostAsync("Ok, you will need to provide some details to create an incident.");

                var incidentForm = (IDialog<Incident>)FormDialog.FromForm(Incident.BuildForm, FormOptions.PromptInStart);
                context.Call(incidentForm, OnCreateIncidentComplete);
            }
            catch
            {
                await context.PostAsync("I'm sorry but something happened. Please, try again later on.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("GetAllIncidents")]
        public async Task GetAllIncidents(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Here are your incidents:");
            string incidentsReply = string.Empty;
            var incidentsCount = this.incidents.Count;
            for (int i = 0; i < incidentsCount; i++)
            {
                incidentsReply += $"{i}: INCIDENT: {incidents[i].Description}, URGENCY: {incidents[i].Urgency} \n";
            }

            await context.PostAsync(incidentsReply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetIncidentState")]
        public async Task GetIncident(IDialogContext context, LuisResult result)
        {
            var incidentId = string.Empty;
            if (result.Entities.Count > 0)
            {
                incidentId = result.Entities.FirstOrDefault(e => e.Type == "IncidentID").Entity;
            }

            if (string.IsNullOrEmpty(incidentId))
            {
                PromptDialog.Text(
                    context: context, 
                    resume: OnIncidentIdPromptComplete, 
                    prompt: "What's the ID of the incident you want me to check?",
                    retry: "Sorry, I didn't understant that. Please try again.");
            }
            else
            {
                // Get incident
                var incident = this.incidents.Where(i => i.ID.ToLower() == incidentId.ToLower()).FirstOrDefault();

                if (incident == null)
                {
                    await context.PostAsync($"No incident with Id {incidentId} has been found.");
                }
                else
                {
                    await context.PostAsync($"Incident with Id {incidentId} is {incident.State}.");
                }
            }

            //context.Wait(MessageReceived);
        }

        private async Task OnCreateIncidentComplete(IDialogContext context, IAwaitable<Incident> result)
        {
            try
            {
                // TODO Send created incident to service now
                var incident = await result;

                await context.PostAsync("Successfully created an incident. Stay tuned for updates on your incident.");
                context.Wait(MessageReceived);
            }
            catch (FormCanceledException)
            {
                await context.PostAsync("Don't want to create incident anymore? Ok.");
            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry but something happened. Please, try again later on.");
            }
            finally
            {
                context.Wait(MessageReceived);
            }
        }

        private async Task OnIncidentIdPromptComplete(IDialogContext context, IAwaitable<string> result)
        {
            var incidentId = await result;
            var pattern = @"inc[0-9]{7}";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(incidentId);
            if (match.Success)
            {
                incidentId = match.Value;

                // Get incident
                var incident = this.incidents.Where(i => i.ID.ToLower() == incidentId.ToLower()).FirstOrDefault();

                if (incident == null)
                {
                    await context.PostAsync($"No incident with Id '{incidentId}' has been found.");
                }
                else
                {
                    await context.PostAsync($"Incident with Id '{incidentId}' is {incident.State}.");
                }
            }
            else
            {
                //IMessageActivity res = context.MakeMessage();
                //res.Text = incidentId;

                await base.MessageReceived(context, Awaitable.FromItem(context.Activity.AsMessageActivity()));

                //await base.MessageReceived(context, );
            }
        }
    }
}