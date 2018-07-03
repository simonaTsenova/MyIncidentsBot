# MyIncidentsBot

## Summary
This project represents a chat bot to help employees in a certain company handle incidents in ServiceNow platform. The main idea is to ease and fasten the process of creating and retrieving incidents and allow employees to manage them on the go through Microsoft Teams channel. The bot is build using [Microsoft Bot Framework](https://dev.botframework.com/) and integrates one of Microsoft's Cognitive Services -  [Language Understanding and Intelligence Service (LUIS)](https://www.luis.ai/home) to make the bot intelligent and provide users with more natural conversations. The project is fully written in C#.

## LUIS
For this bot LUIS is trained to recognize four main intents:
* CreateIncident - if this intent is recognized, the bot starts a FormFlow dialog for the user to fill required data to create new incident in ServiceNow portal; the data the bot asks for is short description (plain text) and urgency type (chosen among few options);
* GetLatestIncidents - this intent returns the latest incidents that have been added to ServiceNow;
* GetIncidentState - this intent checks for the state of an incident with provided ID which is marked as entity in LUIS intent; if ID entity is not provided, the user is prompted to provide valid ID and tries to search again;
* GetAllIncidents - this intent should return all incidents;

*There is one more intent that each LUIS app recognizes and has by default. This is the None intent, which means the intent is none of the others provided to LUIS. In the case of this project, when LUIS recognizes None intent, the bot is trying to match the user input to some common responses (like greeting, goodbye, gratefullness) that have been added using BestMatchDialog NuGet package.

## Main flow
The user starts a dialog by typing some input, which triggers the bot to turn to LUIS to recognize user's intent. Once that has happened, a respective method in the dialog is called to take some action according to the desired intent. When LUIS cannot recognize intent it responds with an appropriate message.

## Samples
The samples show some usage.
The following sample shows requesting the state of incident with some ID.
![alt text](https://github.com/simonaTsenova/MyIncidentsBot/blob/master/samples/sample1.png "Sample screenshot 1")
The next screenshot shows few requests for getting latest incidents.
![alt text](https://github.com/simonaTsenova/MyIncidentsBot/blob/master/samples/sample2.png "Sample screenshot 2")
Third sample shows process of creating incident.
![alt text](https://github.com/simonaTsenova/MyIncidentsBot/blob/master/samples/sample3.png "Sample screenshot 3")
