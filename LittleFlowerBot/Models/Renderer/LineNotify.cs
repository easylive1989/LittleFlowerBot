using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using LittleFlowerBot.Repositories;
using Microsoft.Extensions.Configuration;

namespace LittleFlowerBot.Models.Renderer
{
    public class LineNotify : ITextRenderer, ILineNotify
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISubscriptionRepository _subscriptionRepository;

        private readonly string _notifyApi = "https://notify-api.line.me/api/notify";

        public LineNotify(IHttpClientFactory httpClientFactory, ISubscriptionRepository subscriptionRepository)
        {
            _httpClientFactory = httpClientFactory;
            _subscriptionRepository = subscriptionRepository;
        }

        public void Render(string to, string text)
        {
            var receiver = _subscriptionRepository.Get(to).Receiver;
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