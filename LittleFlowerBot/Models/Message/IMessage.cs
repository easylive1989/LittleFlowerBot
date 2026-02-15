using System.Collections.Generic;

namespace LittleFlowerBot.Models.Message
{
    public interface IMessage
    {
        void Reply(string replyToken, string text, List<QuickReplyItem>? quickReplyItems = null);
    }
}