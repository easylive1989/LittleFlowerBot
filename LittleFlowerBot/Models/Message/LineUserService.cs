using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LittleFlowerBot.Models.Message
{
    public class LineUserService : ILineUserService
    {
        private readonly string _channelToken;
        private readonly IHttpClientFactory _httpClientFactory;

        public LineUserService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _channelToken = configuration.GetValue<string>("LineChannelToken") ?? string.Empty;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> IsFollower(string userId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.line.me/v2/bot/profile/{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _channelToken);

            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
