# Exploration of Elastic integration with Azure Functions

This is purely exploration code as a sample of the type of Azure Functions integration you can write, no indication of planned functionality, and comes with no support or warranty of any kind!

* Elastic.FunctionBinding - project with integration implementation
* ElasticFunctionDemoCSharp - C# demo project for integration
* ElasticFunctionDemoJavaScript - JavaScript demo project for integration

## Misc notes

Use the `SendMessageToEventHub.ps1` script to send messages to Event Hub. This uses the connection configured in the Functions config (`local.settings.json`):

```powershell
1..10 |%{ .\SendMessageToEventHub.ps1 -message "{ 'name':'bob$_','value': 1}" }
1..5 |%{ .\SendMessageToEventHub.ps1 -message "{ 'name':'bob$_','value': 2}" }
```

In the Kibana Dev Console, delete the index to easily reset for demoing/testing:

```HTTP
DELETE /people
```

Config (local.settings.json):

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ElasticUrl": "http://elasticsearch:9200",
    "ElasticUsername": "elastic",
    "ElasticPassword": "YourPassword",
    "EventHubConnection": "Endpoint=sb://YourNamespace.servicebus.windows.net/;SharedAccessKeyName=YourKeyName;SharedAccessKey=YourKey;EntityPath=YourEventHubName"
  },
  "ConnectionStrings": {}
}
```
