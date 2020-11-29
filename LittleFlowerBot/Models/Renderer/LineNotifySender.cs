using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Renderer
{
    public class LineNotifySender : ITextRenderer
    {
        public string SenderId { get; set; }
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISubscriptionRepository _subscriptionRepository;

        private readonly string _notifyApi = "https://notify-api.line.me/api/notify";

        public LineNotifySender(IHttpClientFactory httpClientFactory, ISubscriptionRepository subscriptionRepository)
        {
            _httpClientFactory = httpClientFactory;
            _subscriptionRepository = subscriptionRepository;
        }

        public void Render(string text)
        {
            var receiver = _subscriptionRepository.Get(SenderId).Receiver;
            var requestMessage =
                new HttpRequestMessage(HttpMethod.Post, _notifyApi)
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("message", $"\n{text}")
                    })
                };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", receiver);

            _httpClientFactory.CreateClient().SendAsync(requestMessage);
        }
    }
}