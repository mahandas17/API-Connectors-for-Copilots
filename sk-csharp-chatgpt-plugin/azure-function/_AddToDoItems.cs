using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ToDo
{
    public class _AddToDoItems
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger _logger;
        private string ToDo_API_URL = "https://localhost:7003/todoitems/";
        public _AddToDoItems(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<_GetToDoItems>();
        }

        [OpenApiOperation(operationId: "CreateTodo", tags: new[] { "ExecuteFunction" }, Description = "Creates a new to do item in the to do list")]
        [OpenApiParameter(name: "name", Description = "The name of the task", Required = true, In = ParameterLocation.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(TodoItem[]), Description = "Returns back the confirmation of task.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Displays an error message.")]
        [Function("CreateTodo")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            string name = req.Query["name"];

            await AddItem(name).ConfigureAwait(false);

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "text/plain");

            response.WriteString("Item added to the list.");

            _logger.LogInformation($"Item added to the list.");

            return response;
        }
        private async Task AddItem(string name)
        {
            var url = ToDo_API_URL;

            var body = new TodoItem() { Id = Guid.NewGuid(), Name = name, IsComplete = false };

            var requestData = JsonConvert.SerializeObject(body).ToString();

            string json = JsonConvert.SerializeObject(body);

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, httpContent).ConfigureAwait(false);

            return;
        }

    }
}
