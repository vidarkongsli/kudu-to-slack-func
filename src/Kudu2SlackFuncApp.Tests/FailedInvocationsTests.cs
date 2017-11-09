using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Kudu2SlackFuncApp.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kudu2SlackFuncApp.Tests
{
    [TestClass]

    public class FailedInvocationTestsBase : Kudu2SlackRelayTestsBase
    {
        [TestMethod]
        public void ShouldFailWhenNoWebHookGiven()
        {
            var req = CreateRequestFrom(SuccessfulKuduNotification, new Dictionary<string, string>
            {
                { "channel", "deployment"}
            });

            var resp = Kudu2SlackRelay.Run(req, new NullTraceWriter(TraceLevel.Off)).GetAwaiter().GetResult() as HttpResponseMessage;
            Assert.IsNotNull(resp, "Got a null response from function");
            Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [TestMethod]
        public void ShouldFailWhenNoChannelGiven()
        {
            var req = CreateRequestFrom(SuccessfulKuduNotification, new Dictionary<string, string>
            {
                { "webhook", "https://foo.com/bar" }
            });

            var resp = Kudu2SlackRelay.Run(req, new NullTraceWriter(TraceLevel.Off)).GetAwaiter().GetResult() as HttpResponseMessage;
            Assert.IsNotNull(resp, "Got a null response from function");
            Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}
