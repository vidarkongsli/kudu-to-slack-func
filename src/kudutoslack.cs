using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace v2
{
    public static class KuduToSlack
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .Register(services => services.AddSingleton<HttpClient>())
            .Build();

        [FunctionName("kudutoslack")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var webhook = req.Query[webhookParameter].ToString();
            if (string.IsNullOrWhiteSpace(webhook))
            {
                return new BadRequestObjectResult($"Parameter '{webhookParameter}' is missing");
            }
            var channel = req.Query[channelParameter].ToString();
            if (string.IsNullOrWhiteSpace(channel))
            {
                return new BadRequestObjectResult($"Parameter '{channelParameter}' is missing");
            }

            var incomingJsonData = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Payload: {incomingJsonData}");

            if (string.IsNullOrWhiteSpace(incomingJsonData))
            {
                return new BadRequestObjectResult("No payload found in the request body");
            }

            var slackMessage =
                JsonConvert.DeserializeObject<KuduDeployResult>(incomingJsonData)
                    .Map(SlackMessage.From)
                    .ToChannel(channel)
                    .Map(JsonConvert.SerializeObject);

            log.LogInformation($"Payload {incomingJsonData} transformed to {slackMessage}");

            var request = slackMessage
                .Map(x => new StringContent(x, Encoding.UTF8, "application/json"))
                .Map(x => new HttpRequestMessage(HttpMethod.Post, webhook) {
                    Content = x
                });

            var httpResponse = await Container
                .GetService<HttpClient>()
                .SendAsync(request, CancellationToken.None);

            if (httpResponse.IsSuccessStatusCode)
            {
                return new OkResult();
            }
            var errorMessage = $@"Error processing, upstream webhook returned: {httpResponse.StatusCode} ({httpResponse.ReasonPhrase})";
            log.LogError(errorMessage);
            return new BadRequestObjectResult(errorMessage);
        }

        private const string webhookParameter = "webhook";

        private const string channelParameter = "channel";
    }
}
