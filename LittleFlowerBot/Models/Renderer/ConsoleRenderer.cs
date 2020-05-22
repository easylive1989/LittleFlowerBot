using System;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Message;

namespace LittleFlowerBot.Models.Renderer
{
    public class ConsoleRenderer : ITextRenderer, IMessage, ILineNotify
    {
        public void Render(string to, string text)
        {
            Console.WriteLine(text);
        }

        public void Reply(string replyToken, string text)
        {
            Console.Write(text);
        }

        public Task SaveToken(string senderId, string code)
        {
            Console.Write(code);
            return Task.CompletedTask;
        }

        public string GenerateLink(string guid)
        {
            return guid;
        }

        public bool IsRegistered(string senderId)
        {
            return false;
        }
    }
}