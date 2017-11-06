using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Kudu2SlackFuncApp
{
    public static class Kudu2SlackRelay
    {
        [FunctionName("Kudu2SlackRelay")]
        public static async Task<object> Run(
            [HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            var queryString = req.RequestUri.ParseQueryString();

            const string webhookParameter = "webhook";
            var outgoingWebHook = queryString.Get(webhookParameter);
            if (outgoingWebHook == null)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, $"Parameter '{webhookParameter}' is missing");
            }

            const string channelParameter = "channel";
            var channel = queryString.Get(channelParameter);
            if (channel == null) {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, $"Parameter '{channelParameter}' is missing");
            }

            var incomingJsonData = await req.Content.ReadAsStringAsync();

            log.Info($"Payload: {incomingJsonData}");

            var slackMessage =
                JsonConvert.DeserializeObject<KuduDeployResult>(incomingJsonData)
                    .Map(SlackMessage.From)
                    .ToChannel(channel)
                    .Map(JsonConvert.SerializeObject)
                    .Map(x => new StringContent(x, Encoding.UTF8, "application/json"));

            var httpResponse = await _client.PostAsync(outgoingWebHook, slackMessage).ConfigureAwait(false);
            if (httpResponse.IsSuccessStatusCode)
            {
                return req.CreateResponse(HttpStatusCode.OK);
            }
            log.Error($@"Error processing {incomingJsonData} => {httpResponse.StatusCode} ({httpResponse.ReasonPhrase})");
            return req.CreateResponse(httpResponse.StatusCode);
        }

        private static HttpClient _client;

        static Kudu2SlackRelay()
        {
            SetHttpClient(new HttpClient());
        }

        public static void SetHttpClient(HttpClient client)
        {
            _client = client;
        }
    }
}
