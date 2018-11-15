
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Nest;
using System;
using Elastic.FunctionBinding;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ElasticFunctionDemoCSharp
{
    public static class Function1
    {
        private const string Password = "PasswordHere";
        private const string ElasticEndpoint = "http://elastic:9200";

        [FunctionName("HttpManual")]
        public static IActionResult HttpManual(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var settings = new ConnectionSettings(new Uri(ElasticEndpoint))
                                     .BasicAuthentication("elastic", Password)
                                     .DefaultIndex("people");

            var client = new ElasticClient(settings);

            var response = client.IndexDocument(new Wibble { Name = name, Value = 10 });

            if (response.IsValid)
            {
                log.Info("Success");
            }
            else
            {
                log.Error($"Failed. {response.ServerError.ToString()}");
            }


            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string");
        }

        // Config similar to:
        //{
        //  "IsEncrypted": false,
        //  "Values": {
        //    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        //    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        //    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        //    "ElasticUrl": "http://elastic:9200",
        //    "ElasticUsername": "elastic",
        //    "ElasticPassword": "PasswordHere",
        //  },
        //  "ConnectionStrings": {}
        //}
        [FunctionName("HttpIntegrated")]
        public static IActionResult HttpIntegrated(
                [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
                HttpRequest req,
                ILogger log,
                [Elastic(Index ="people", IndexType = "wibble")]
                out ElasticMessage<Wibble> elasticMessage)
        {
            log.LogInformation("C# HTTP trigger function2 processed a request.");

            string name = req.Query["name"];

            elasticMessage = new ElasticMessage<Wibble>(new Wibble { Name = name, Value = 20 });

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello2, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string");
        }

        [FunctionName("HttpIntegratedAsync")]
        public static async Task<IActionResult> HttpIntegratedAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            [Elastic(Index ="people", IndexType = "wibble")]
            IAsyncCollector<IElasticMessage> elasticMessage)
        {
            log.LogInformation("C# HTTP trigger function2 processed a request.");

            string name = req.Query["name"];

            await elasticMessage.AddAsync(new ElasticMessage<Wibble>(new Wibble { Name = name, Value = 20 }));

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello2, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string");
        }


        [FunctionName("HttpPocoIntegrated")]
        public static IActionResult HttpPocoIntegrated(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
                HttpRequest req,
                ILogger log,
                [Elastic(Index ="people", IndexType = "wibble")]
                out Wibble wibble)
        {
            log.LogInformation("C# HTTP trigger function2 processed a request.");

            string name = req.Query["name"];

            wibble = new Wibble { Name = name, Value = 20 };

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello2, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string");
        }

        [FunctionName("HttpPocoIntegratedAsync")]
        public static async Task<IActionResult> HttpPocoIntegratedAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            [Elastic(Index ="people", IndexType = "wibble")]
            IAsyncCollector<Wibble> wibbles)
        {
            log.LogInformation("C# HTTP trigger function2 processed a request.");

            string name = req.Query["name"];

            await wibbles.AddAsync(new Wibble { Name = name, Value = 20 });

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello2, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string");
        }



        [FunctionName("EventHub")]
        public static async Task EventHub(
            [EventHubTrigger("sl-estest", Connection = "EventHubConnection")]
            Wibble[] wibbles,
            ILogger log,
            [Elastic(Index ="people", IndexType = "wibble")]
            IAsyncCollector<IElasticMessage> elasticMessages
        )
        {
            foreach (var wibble in wibbles)
            {
                log.LogInformation($"C# Event Hub trigger function processed a message: {wibble.Name}, {wibble.Value}");

                await elasticMessages.AddAsync(new ElasticMessage<Wibble>(wibble));
            }
        }

    }

    [ElasticsearchType(IdProperty = nameof(Name))]
    public class Wibble
    {
        public Wibble()
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
        }

        public string Name { get; set; }

        public int Value { get; set; }

        public string Timestamp { get; set; }
    }
}