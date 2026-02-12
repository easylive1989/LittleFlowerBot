using System;
using LittleFlowerBot.Models.Message;

namespace LittleFlowerBot.Models.Renderer
{
    public class ConsoleRenderer : ITextRenderer, IMessage
    {
        public void Render(string text)
        {
            Console.WriteLine(text);
        }

        public void Reply(string replyToken, string text)
        {
            Console.Write(text);
        }
    }
}
