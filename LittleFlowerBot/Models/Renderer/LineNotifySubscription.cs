using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Exceptions;
using LittleFlowerBot.Models.Responses;
using LittleFlowerBot.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LittleFlowerBot.Models.Renderer
{
    public class LineNotifySubscription : ILineNotifySubscription
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISubscriptionRepository _subscriptionRepository;

        private readonly string _tokenApi = "https://notify-bot.line.me/oauth/token";
        private readonly string _lineNotifyClientId;
        private readonly string _lineNotifyClientSecret;

        public LineNotifySubscription(IHttpClientFactory httpClientFactory, ISubscriptionRepository subscriptionRepository,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _subscriptionRepository = subscriptionRepository;
            _lineNotifyClientId = configuration.GetValue<string>("LineNotifyClientId");
            _lineNotifyClientSecret = configuration.GetValue<string>("LineNotifyClientSecret");
        }

        public async Task SaveToken(string senderId, string code)
        {
            var request =
                new HttpRequestMessage(HttpMethod.Post, _tokenApi)
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("redirect_uri",
                            "https://littleflowerbot.herokuapp.com/api/LineChat/Notify"),
                        new KeyValuePair<string, string>("client_id", _lineNotifyClientId),
                        new KeyValuePair<string, string>("client_secret", _lineNotifyClientSecret),
                        new KeyValuePair<string, string>("code", code)
                    })
                };

            var response = (await _httpClientFactory.CreateClient().SendAsync(request));
            var content = await response.Content.ReadAsStringAsync();
            var lineNotifyTokenResponse = JsonConvert
                .DeserializeObject<LineNotifyTokenResponse>(content);
            if (lineNotifyTokenResponse.Status != (int)HttpStatusCode.OK)
            {
                throw new LineNotifyTokenInvalidException(lineNotifyTokenResponse.Message);
            }

            await _subscriptionRepository.AddAsync(senderId, lineNotifyTokenResponse.Access_Token);
        }

        public string GenerateLink(string guid)
        {
            return
                $"https://notify-bot.line.me/oauth/authorize?response_type=code&scope=notify&response_mode=form_post&client_id={_lineNotifyClientId}&redirect_uri=https://littleflowerbot.herokuapp.com/api/LineChat/Notify&state={guid}";
        }

        public bool IsRegistered(string senderId)
        {
            return _subscriptionRepository.IsExist(senderId);
        }
    }
}