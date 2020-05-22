using isRock.LineBot;
using LittleFlowerBot.Models.Renderer;
using Microsoft.Extensions.Configuration;

namespace LittleFlowerBot.Models.Message
{
    public class LineMessage : IMessage
    {
        private readonly string _channelToken;

        public LineMessage(IConfiguration configuration)
        {
            _channelToken = configuration.GetValue<string>("LineChannelToken");
        }

        public void Reply(string replyToken, string text)
        {
            Utility.ReplyMessage(replyToken, text, _channelToken);
        }
    }
}