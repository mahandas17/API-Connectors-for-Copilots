using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SecurityPlugin
{
    public class IdentifyAPI
    {
        private readonly ILogger _logger;

        public IdentifyAPI(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<IdentifyAPI>();
        }

        [OpenApiOperation(operationId: "GetAPI", tags: new[] { "ExecuteFunction" }, Description = "Identify API based on the intent.")]
        [OpenApiParameter(name: "intent", Description = "The user intent", Required = true, In = ParameterLocation.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns back the API url.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Displays an error message.")]
        [Function("IdentifyAPI")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            string jintent = req.Query["intent"];
            string URL = "https://graph.microsoft.com/v1.0/";
            var jobject = JsonConvert.DeserializeObject<dynamic>(jintent);

            switch (jobject.ToString())
            {
                case "SecuirtyIsight":
                    URL = URL + "security/";
                    break;

                case "EdgeInsight":
                    URL = URL + "admin/edge/internetExplorerMode/";
                    break;

                case "ComplianceInsight":
                    URL = URL + "beta/compliance/ediscovery/";
                    break;

            }

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "text/plain");

            response.WriteString(URL);

            _logger.LogInformation($"Item added to the list.");

            return response;
        }


    }
}
