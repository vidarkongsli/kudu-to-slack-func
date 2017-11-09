using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Kudu2SlackFuncApp.Tests.Helpers
{
    public class Kudu2SlackRelayTestsBase
    {

        protected static HttpRequestMessage CreateRequestFrom(string json, IDictionary<string, string> queryParameters) => new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://localhost?" + queryParameters
                                     .Select(kv => $"{kv.Key}={kv.Value}")
                                     .Aggregate("", (a, b) => $"{a}&{b}")),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }.Tee(r => r.SetConfiguration(new HttpConfiguration()));

        protected const string SuccessfulKuduNotification = @"{
  ""id"": ""982843aff56d37f2bfb9f532a9c0465031f4172d"",
            ""status"": ""success"",
            ""statusText"": """",
            ""authorEmail"": ""someone@somewhere.com"",
            ""author"": ""Someone"",
            ""message"": ""My fix"",
            ""deployer"": ""Someone"",
            ""receivedTime"": ""2015-12-16T00:52:07.7240633Z"",
            ""startTime"": ""2016-02-27T17:44:25.8567966Z"",
            ""endTime"": ""2016-02-27T17:45:06.2016537Z"",
            ""lastSuccessEndTime"": ""2016-02-27T17:45:06.2016537Z"",
            ""complete"": true,
            ""siteName"": ""MySite"",
            ""hostName"": ""MySite(-soltName).scm.azurewebsites.net""
        }";

        protected const string FailedKuduNotification = @"{
  ""id"": ""982843aff56d37f2bfb9f532a9c0465031f4172d"",
            ""status"": ""failed"",
            ""statusText"": """",
            ""authorEmail"": ""someone@somewhere.com"",
            ""author"": ""Someone"",
            ""message"": ""My fix"",
            ""deployer"": ""Someone"",
            ""receivedTime"": ""2015-12-16T00:52:07.7240633Z"",
            ""startTime"": ""2016-02-27T17:44:25.8567966Z"",
            ""endTime"": ""2016-02-27T17:45:06.2016537Z"",
            ""lastSuccessEndTime"": ""2016-02-27T17:45:06.2016537Z"",
            ""complete"": true,
            ""siteName"": ""MySite"",
            ""hostName"": ""MySite(-soltName).scm.azurewebsites.net""
        }";
    }
}