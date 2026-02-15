using System.Collections.Generic;
using LittleFlowerBot.Models.Message;

namespace LittleFlowerBot.Models.Renderer
{
    public class BufferedReplyRenderer : ITextRenderer
    {
        private readonly IMessage _message;
        private readonly List<string> _buffer = new();

        public string? ReplyToken { get; set; }
        public List<QuickReplyItem>? QuickReplyItems { get; set; }

        public BufferedReplyRenderer(IMessage message)
        {
            _message = message;
        }

        public void Render(string text)
        {
            _buffer.Add(text);
        }

        public void Flush()
        {
            if (_buffer.Count == 0 || string.IsNullOrEmpty(ReplyToken))
            {
                return;
            }

            var joinedText = string.Join("\n", _buffer);
            _message.Reply(ReplyToken, joinedText, QuickReplyItems);
            _buffer.Clear();
            QuickReplyItems = null;
        }
    }
}
