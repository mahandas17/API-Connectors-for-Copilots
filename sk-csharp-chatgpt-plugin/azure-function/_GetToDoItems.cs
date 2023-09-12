using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ToDo
{
    public class TodoItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }
        [JsonProperty(PropertyName = "isComplete")]
        public bool IsComplete { get; set; }

    }
    public class _GetToDoItems
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger _logger;
        private string ToDo_API_URL = "https://localhost:7003/todoitems/";
        public _GetToDoItems(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<_GetToDoItems>();
        }

        [OpenApiOperation(operationId: "GetToDoItems", tags: new[] { "ExecuteFunction" }, Description = "Gets the tasks of the current user. This function returns a string with a list of tasks in it.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the list of todo items with its completion status.")]
        [Function("GetToDoItems")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            List<TodoItem> items = await FetchItems().ConfigureAwait(false);

            if (items != null)
            {
                List<TodoItem> InCompleteItems = items.Where(a => a.IsComplete == false).ToList();

                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

                response.Headers.Add("Content-Type", "application/json");

                var jsonToReturn = JsonConvert.SerializeObject(items);

                response.WriteString(jsonToReturn);

                return response;

            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

                response.Headers.Add("Content-Type", "application/json");

                response.WriteString("No todo items in the list.");

                _logger.LogInformation($"No todo items in the list.");

                return response;
            }


        }
        private async Task<List<TodoItem>> FetchItems()
        {
            var url = ToDo_API_URL;

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            List<TodoItem> responseData = JsonConvert.DeserializeObject<List<TodoItem>>(json);

            return responseData;

        }

    }
}
