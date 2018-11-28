using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using v2;
using Xunit;

namespace tests
{
    public class KuduToSlackTests
    {
        private const string WebHookUrl = "https://foo.bar";

        private MockedHttpContext _httpContext = MockedHttpContext.Build()
            .WithRequestMethod("POST");

        private readonly MockHttpMessageHandler _mockHttp = new MockHttpMessageHandler();

        public KuduToSlackTests()
        {
            KuduToSlack.Container = new ContainerBuilder()
                .Register(s => s.AddSingleton<HttpClient>(_ => new HttpClient(_mockHttp)))
                .Build();
        }

        [Fact]
        public async Task ShouldWorkSuccessfully()
        {
            _httpContext
                .WithQueryParameter("webhook", WebHookUrl)
                .WithQueryParameter("channel", "channel")
                .WithBody(KuduPayload);

            _mockHttp.When(HttpMethod.Post, WebHookUrl)
                .Respond(HttpStatusCode.OK);

            var result = await RunFunction();
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestIfWebHookParameterIsMissing()
        {
            _httpContext
                .WithQueryParameter("channel", "channel")
                .WithBody(KuduPayload);

            var result = await RunFunction();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

       [Fact]
        public async Task ShouldReturnBadRequestIfChannelParameterIsMissing()
        {
            _httpContext
                .WithQueryParameter("webhook", WebHookUrl)
                .WithBody(KuduPayload);

            var result = await RunFunction();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestIfPayloadNotCorrect()
        {
            _httpContext
                .WithQueryParameter("webhook", WebHookUrl)
                .WithQueryParameter("channel", "channel")
                .WithBody(@"{""foo"":""bar""}");

            var result = await RunFunction();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestQhenBackendWebhookFailed()
        {
            _httpContext
                .WithQueryParameter("webhook", WebHookUrl)
                .WithQueryParameter("channel", "channel")
                .WithBody(KuduPayload);

            _mockHttp.When(HttpMethod.Post, WebHookUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var result = await RunFunction();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private Task<IActionResult> RunFunction() => KuduToSlack.Run(_httpContext.Request, NullLogger.Instance);

        private const string KuduPayload = @"{
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
  ""hostName"": ""MySite(-slotName).scm.azurewebsites.net""
}";
    }
}
