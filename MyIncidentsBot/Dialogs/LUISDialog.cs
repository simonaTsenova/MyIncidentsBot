using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using MyIncidentsBot.Models;
using MyIncidentsBot.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyIncidentsBot.Dialogs
{
    [LuisModel("8b1f49d2-67d0-4ce7-871c-87ce3bf46c38", "166d2616418f4c7cba18bac99764777a")]
    [Serializable]
    public class LUISDialog : LuisDialog<object>
    {
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            var dialog = new CommonResponsesDialog();
            dialog.InitialMessage = result.Query;

            context.Call(dialog, OnCommonResponseHandled);
        }

        [LuisIntent("CreateIncident")]
        public async Task CreateIncident(IDialogContext context, LuisResult result)
        {
            try
            {
                await context.PostAsync("Ok, you will need to provide some details to create an incident.");

                var incidentForm = (IDialog<IncidentForm>)FormDialog.FromForm(IncidentForm.BuildForm, FormOptions.PromptInStart);
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
            await context.PostAsync("I'm sorry, but right now I cannot get all incidents.");
            await context.PostAsync("You can ask me about the latest incidents if you want.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("GetLatestIncidents")]
        public async Task GetLatestIncidents(IDialogContext context, LuisResult result)
        {
            string incidentsReply = string.Empty;
            var incidentsService = new IncidentsService();
            var incidents = await incidentsService.GetLatestIncidents();
            var incidentsCount = incidents.Count;
            if (incidentsCount > 0)
            {
                for (int i = 0; i < incidentsCount; i++)
                {
                    incidentsReply += $"**{i+1}**: **ID**: {incidents[i].Number}, **DESCRIPTION**: {incidents[i].Short_Description}, **URGENCY**: {incidents[i].Urgency}, **STATE**: {incidents[i].State} \n";
                }

                await context.PostAsync("Here are latest incidents:");
                await context.PostAsync(incidentsReply);
            }
            else
            {
                await context.PostAsync("Sorry, didn't find any incidents.");
            }
            
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
                var incidentsService = new IncidentsService();
                var incident = await incidentsService.GetIncidentById(incidentId);

                if (incident == null)
                {
                    await context.PostAsync($"No incident with Id **{incidentId}** has been found.");
                }
                else
                {
                    await context.PostAsync($"State of incident with Id **{incidentId}** is **{incident.State}**.");
                }
            }

            //context.Wait(MessageReceived);
        }

        private async Task OnCommonResponseHandled(IDialogContext context, IAwaitable<bool> result)
        {
            var isMessageHandled = await result;
            if (!isMessageHandled)
            {
                await context.PostAsync("I'm sorry. I didn't understand you.");
            }

            context.Wait(MessageReceived);
        }

        private async Task OnCreateIncidentComplete(IDialogContext context, IAwaitable<IncidentForm> result)
        {
            try
            {
                var incident = await result;

                var incidentsService = new IncidentsService();
                var createdIncidentId = await incidentsService.AddIncident(incident);

                await context.PostAsync($"Successfully created an incident with ID: **{createdIncidentId}**. Stay tuned for updates on your incident.");
            }
            catch (FormCanceledException)
            {
                await context.PostAsync("Don't want to create incident anymore? Ok.");
            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry but something happened. I couldn't create an incident.");
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
                var incidentsService = new IncidentsService();
                var incident = await incidentsService.GetIncidentById(incidentId);

                if (incident == null)
                {
                    await context.PostAsync($"No incident with Id **{incidentId}** has been found.");
                }
                else
                {
                    await context.PostAsync($"State of incident with Id **{incidentId}** is {incident.State}.");
                }
            }
            else
            {
                //IMessageActivity res = context.MakeMessage();
                //res.Text = incidentId;

                await base.MessageReceived(context, Awaitable.FromItem(context.Activity.AsMessageActivity()));
            }
        }
    }
}