using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LittleFlowerBot.Models.Message
{
    public class LineMessage : IMessage
    {
        private readonly string _channelToken;
        private readonly IHttpClientFactory _httpClientFactory;

        public LineMessage(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _channelToken = configuration.GetValue<string>("LineChannelToken") ?? string.Empty;
            _httpClientFactory = httpClientFactory;
        }

        public void Reply(string replyToken, string text, List<QuickReplyItem>? quickReplyItems = null)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var message = new Dictionary<string, object>
            {
                { "type", "text" },
                { "text", text }
            };

            if (quickReplyItems != null && quickReplyItems.Count > 0)
            {
                message["quickReply"] = new
                {
                    items = quickReplyItems.Select(item => new
                    {
                        type = "action",
                        action = new
                        {
                            type = "message",
                            label = item.Label,
                            text = item.Text
                        }
                    }).ToArray()
                };
            }

            var requestBody = new
            {
                replyToken,
                messages = new[] { message }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/v2/bot/message/reply")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _channelToken);

            var response = httpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ReplyMessage API ERROR: {responseBody}");
            }
        }
    }
}
