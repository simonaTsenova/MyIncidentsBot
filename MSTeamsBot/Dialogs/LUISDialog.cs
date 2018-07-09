﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using MSTeamsBot.Models;
using MSTeamsBot.Common;
using MSTeamsBot.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using MSTeamsBot.Common.Exceptions;
using System.Text.RegularExpressions;

namespace MSTeamsBot.Dialogs
{
    [Serializable]
    [LuisModel(Constants.LUIS_MODEL_ID, Constants.LUIS_SUBSCRIPTION_KEY)]
    public class LUISDialog : LuisDialog<object>
    {
        private readonly IIncidentsService incidentsService;
        private readonly CommonResponsesDialog commonResponsesDialog;

        public LUISDialog(IIncidentsService incidentsService, CommonResponsesDialog commonResponsesDialog)
        {
            this.incidentsService = incidentsService;
            this.commonResponsesDialog = commonResponsesDialog;
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            await context.Forward(this.commonResponsesDialog, OnCommonResponseHandled, await message);
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
            try
            {
                var numberEntity = result.Entities.FirstOrDefault(e => e.Type == "builtin.number");
                var latestIncidentsCount = numberEntity != null ? numberEntity.Entity : Constants.LATEST_INCIDENTS_COUNT;

                var incidents = await this.incidentsService.GetLatestIncidents(latestIncidentsCount);
                var incidentsCount = incidents.Count;
                if (incidentsCount > 0)
                {
                    string incidentsReply = string.Empty;
                    for (int i = 0; i < incidentsCount; i++)
                    {
                        incidentsReply += $"**{i + 1}**: **ID**: {incidents[i].Number}, **DESCRIPTION**: {incidents[i].Short_Description}, **URGENCY**: {incidents[i].Urgency}, **STATE**: {incidents[i].State} \n";
                    }

                    await context.PostAsync("Here are latest incidents:");
                    await context.PostAsync(incidentsReply);
                }
                else
                {
                    await context.PostAsync("Sorry, didn't find any incidents.");
                }
            }
            catch (AuthenticationFailedException)
            {
                await context.PostAsync("I'm sorry but access to ServiceNow was denied. I couldn't get any incidents.");
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

        [LuisIntent("GetIncidentState")]
        public async Task GetIncident(IDialogContext context, LuisResult result)
        {
            try
            {
                var incidentIdEntity = result.Entities.FirstOrDefault(e => e.Type == "IncidentID");

                if (incidentIdEntity == null)
                {
                    PromptDialog.Text(
                        context: context,
                        resume: OnIncidentIdPromptComplete,
                        prompt: "What's the ID of the incident you want me to check?",
                        retry: "Sorry, I didn't understant that. Please try again.");
                }
                else
                {
                    var incidentId = incidentIdEntity.Entity;
                    var incident = await this.incidentsService.GetIncidentById(incidentId);

                    if (incident == null)
                    {
                        await context.PostAsync($"No incident with Id **{incidentId}** has been found.");
                    }
                    else
                    {
                        await context.PostAsync($"State of incident with Id **{incidentId}** is **{incident.State}**.");
                    }
                }
            }
            catch (AuthenticationFailedException)
            {
                await context.PostAsync("I'm sorry but access to ServiceNow was denied. I couldn't get any incidents.");
            }
            catch
            {
                await context.PostAsync("I'm sorry but something happened. Please, try again later on.");
                context.Wait(MessageReceived);
            }
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

                var createdIncidentId = await this.incidentsService.AddIncident(incident);

                await context.PostAsync($"Successfully created an incident with ID: **{createdIncidentId}**. Stay tuned for updates on your incident.");
            }
            catch (FormCanceledException)
            {
                await context.PostAsync("Don't want to create incident anymore? Ok.");
            }
            catch (AuthenticationFailedException)
            {
                await context.PostAsync("I'm sorry but access to ServiceNow was denied. I couldn't create an incident.");
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
            try
            {
                var userResponse = await result;
                var incidentIdPattern = @"inc[0-9]{7}";
                var regex = new Regex(incidentIdPattern, RegexOptions.IgnoreCase);
                var match = regex.Match(userResponse);
                if (match.Success)
                {
                    var incidentId = match.Value;
                    var incident = await this.incidentsService.GetIncidentById(incidentId);

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
                    await base.MessageReceived(context, Awaitable.FromItem(context.Activity.AsMessageActivity()));
                }
            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry but something happened. Please, try again later on.");
            }
        }
    }
}