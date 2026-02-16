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

        public void RenderImage(byte[] imageData)
        {
            Console.WriteLine($"[Board Image: {imageData.Length} bytes]");
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

        public void ReplyMessages(string replyToken, List<ReplyMessageItem> messages, List<QuickReplyItem>? quickReplyItems = null)
        {
            foreach (var msg in messages)
            {
                switch (msg)
                {
                    case TextReplyMessage textMsg:
                        Console.Write(textMsg.Text);
                        break;
                    case ImageReplyMessage imageMsg:
                        Console.Write($"[Image: {imageMsg.ImageUrl}]");
                        break;
                }
            }

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
