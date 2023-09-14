using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;

public class AIPluginJson
{
    [Function("GetAIPluginJson")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/ai-plugin.json")] HttpRequestData req)
    {
        var currentDomain = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}";

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        var appSettings = AppSettings.LoadSettings();

        // serialize app settings to json using System.Text.Json
        var json = System.Text.Json.JsonSerializer.Serialize(appSettings.AIPlugin);

        // replace {url} with the current domain
        json = json.Replace("{url}", currentDomain, StringComparison.OrdinalIgnoreCase);

        response.WriteString(json);

        return response;
    }
}
/*
 * [INSTRUCTIONS]
Generate a Microsoft Graph API based Odata query to get the user detail based on the user input below
{{$input}}


[query]
-*------------
{
  "schema": 1,
  "description": "Provides User related Microsoft Graph API endpoint with odata queries based on user input.",
  "type": "completion",
  "completion": {
    "max_tokens": 1000,
    "temperature": 0.0,
    "top_p": 0.0,
    "presence_penalty": 0.0,
    "frequency_penalty": 0.0
  },
  "input": {
    "parameters": [
      {
        "name": "input",
        "description": "A string that contains a user query",
        "defaultValue": ""
      }
    ]
  }
}
*/