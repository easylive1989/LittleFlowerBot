using System;
using System.Collections.Generic;
using LittleFlowerBot.Models.Message;

namespace LittleFlowerBot.Models.Renderer
{
    public class ConsoleRenderer : ITextRenderer, IMessage
    {
        public void Render(string text)
        {
            Console.WriteLine(text);
        }

        public void Reply(string replyToken, string text, List<QuickReplyItem>? quickReplyItems = null)
        {
            Console.Write(text);
            if (quickReplyItems != null)
            {
                foreach (var item in quickReplyItems)
                {
                    Console.Write($"\n[{item.Label}]");
                }
            }
        }
    }
}
