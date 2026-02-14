using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LittleFlowerBot.Models.Message
{
    public class LineMessage : IMessage
    {
        private readonly string _channelToken;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LineMessage> _logger;

        public LineMessage(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<LineMessage> logger)
        {
            _channelToken = configuration.GetValue<string>("LineChannelToken") ?? string.Empty;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public void Reply(string replyToken, string text)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                replyToken,
                messages = new[]
                {
                    new { type = "text", text }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/v2/bot/message/reply")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _channelToken);

            _logger.LogWarning("[DEBUG] Sending LINE reply: token={Token}..., textLength={Length}, channelTokenLength={ChannelTokenLength}",
                replyToken[..Math.Min(8, replyToken.Length)], text.Length, _channelToken.Length);

            var response = httpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("LINE Reply API failed: status={Status}, body={Body}, replyToken={Token}",
                    response.StatusCode, responseBody, replyToken);
                throw new Exception($"ReplyMessage API ERROR: {responseBody}");
            }

            _logger.LogWarning("[DEBUG] LINE reply succeeded: status={Status}", response.StatusCode);
        }
    }
}
