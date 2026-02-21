using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Message;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Message
{
    [TestFixture]
    public class LineUserServiceTests
    {
        private IConfiguration _configuration = null!;
        private IHttpClientFactory _httpClientFactory = null!;
        private FakeHttpMessageHandler _httpHandler = null!;
        private LineUserService _service = null!;

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

            _service = new LineUserService(_configuration, _httpClientFactory);
        }

        [Test]
        public async Task IsFollower_WhenUserIsFollower_ShouldReturnTrue()
        {
            _httpHandler.ResponseStatusCode = HttpStatusCode.OK;

            var result = await _service.IsFollower("user123");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsFollower_WhenUserIsNotFollower_ShouldReturnFalse()
        {
            _httpHandler.ResponseStatusCode = HttpStatusCode.NotFound;

            var result = await _service.IsFollower("user123");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsFollower_ShouldCallProfileApiWithUserId()
        {
            _httpHandler.ResponseStatusCode = HttpStatusCode.OK;

            await _service.IsFollower("user123");

            Assert.That(_httpHandler.LastRequest!.RequestUri!.ToString(),
                Is.EqualTo("https://api.line.me/v2/bot/profile/user123"));
        }

        [Test]
        public async Task IsFollower_ShouldSendBearerToken()
        {
            _httpHandler.ResponseStatusCode = HttpStatusCode.OK;

            await _service.IsFollower("user123");

            Assert.That(_httpHandler.LastRequest!.Headers.Authorization!.Scheme, Is.EqualTo("Bearer"));
            Assert.That(_httpHandler.LastRequest.Headers.Authorization.Parameter, Is.EqualTo("test-token"));
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }
            public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(ResponseStatusCode));
            }
        }
    }
}
