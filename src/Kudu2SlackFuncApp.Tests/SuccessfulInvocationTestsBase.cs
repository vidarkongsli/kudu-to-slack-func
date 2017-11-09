using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Kudu2SlackFuncApp.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Kudu2SlackFuncApp.Tests
{
    [TestClass]
    public class SuccessfulInvocationTestsBase : Kudu2SlackRelayTestsBase
    {
        [TestMethod]
        public void HandleSuccessfulDeployment()
        {
            var req = CreateRequestFrom(SuccessfulKuduNotification, new Dictionary<string, string>
            {
                { "webhook", "https://foo.com/bar" },
                { "channel", "deployment"}
            });
            var stub = new DelegatingHandlerStub();
            Kudu2SlackRelay.SetHttpClient(new HttpClient(stub));

            Kudu2SlackRelay.Run(req, new NullTraceWriter(TraceLevel.Off)).GetAwaiter().GetResult();

            Assert.AreEqual(HttpMethod.Post, stub.RequestMethod);
            var message = JsonConvert.DeserializeObject<SlackMessage>(stub.RequestContent.Result);
            Assert.AreEqual(":shipit:", message.IconEmoji);
            Assert.IsTrue(message.Username.Contains("MySite"));
        }

        [TestMethod]
        public void HandleFailedDeployment()
        {
            var req = CreateRequestFrom(FailedKuduNotification, new Dictionary<string, string>
            {
                { "webhook", "https://foo.com/bar" },
                { "channel", "deployment"}
            });
            var stub = new DelegatingHandlerStub();
            Kudu2SlackRelay.SetHttpClient(new HttpClient(stub));

            Kudu2SlackRelay.Run(req, new NullTraceWriter(TraceLevel.Off)).GetAwaiter().GetResult();

            Assert.AreEqual(HttpMethod.Post, stub.RequestMethod);
            var message = JsonConvert.DeserializeObject<SlackMessage>(stub.RequestContent.Result);
            Assert.AreEqual(":warning:", message.IconEmoji);
            Assert.IsTrue(message.Username.Contains("MySite"));
        }
    }
}
