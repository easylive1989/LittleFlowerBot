using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Message;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Message
{
    [TestFixture]
    public class LineMessageTests
    {
        private IConfiguration _configuration = null!;
        private IHttpClientFactory _httpClientFactory = null!;
        private FakeHttpMessageHandler _httpHandler = null!;
        private LineMessage _lineMessage = null!;

        [SetUp]
        public void Setup()
        {
            _configuration = Substitute.For<IConfiguration>();
            var section = Substitute.For<IConfigurationSection>();
            section.Value.Returns("test-token");
            _configuration.GetSection("LineChannelToken").Returns(section);

            _httpHandler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(_httpHandler);

            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _httpClientFactory.CreateClient().Returns(httpClient);

            _lineMessage = new LineMessage(_configuration, _httpClientFactory);
        }

        [Test]
        public void PushMessages_ShouldCallPushApiEndpoint()
        {
            var messages = new List<ReplyMessageItem> { new TextReplyMessage("hello") };

            _lineMessage.PushMessages("user123", messages);

            Assert.That(_httpHandler.LastRequest!.RequestUri!.ToString(),
                Is.EqualTo("https://api.line.me/v2/bot/message/push"));
        }

        [Test]
        public void PushMessages_ShouldSendUserIdInBody()
        {
            var messages = new List<ReplyMessageItem> { new TextReplyMessage("hello") };

            _lineMessage.PushMessages("user123", messages);

            var body = JsonSerializer.Deserialize<JsonElement>(_httpHandler.LastRequestBody!);
            Assert.That(body.GetProperty("to").GetString(), Is.EqualTo("user123"));
        }

        [Test]
        public void PushMessages_ShouldSendTextMessage()
        {
            var messages = new List<ReplyMessageItem> { new TextReplyMessage("hello") };

            _lineMessage.PushMessages("user123", messages);

            var body = JsonSerializer.Deserialize<JsonElement>(_httpHandler.LastRequestBody!);
            var msgArray = body.GetProperty("messages");
            Assert.That(msgArray[0].GetProperty("type").GetString(), Is.EqualTo("text"));
            Assert.That(msgArray[0].GetProperty("text").GetString(), Is.EqualTo("hello"));
        }

        [Test]
        public void PushMessages_ShouldSendAuthorizationHeader()
        {
            var messages = new List<ReplyMessageItem> { new TextReplyMessage("hello") };

            _lineMessage.PushMessages("user123", messages);

            Assert.That(_httpHandler.LastRequest!.Headers.Authorization!.Scheme, Is.EqualTo("Bearer"));
            Assert.That(_httpHandler.LastRequest.Headers.Authorization.Parameter, Is.EqualTo("test-token"));
        }

        [Test]
        public void Push_ShouldSendSingleTextMessage()
        {
            _lineMessage.Push("user123", "hello");

            var body = JsonSerializer.Deserialize<JsonElement>(_httpHandler.LastRequestBody!);
            Assert.That(body.GetProperty("to").GetString(), Is.EqualTo("user123"));
            var msgArray = body.GetProperty("messages");
            Assert.That(msgArray[0].GetProperty("text").GetString(), Is.EqualTo("hello"));
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestBody { get; private set; }

            protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                LastRequestBody = request.Content?.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Send(request, cancellationToken));
            }
        }
    }
}
