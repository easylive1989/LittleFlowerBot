using System.Collections.Generic;
using LittleFlowerBot.Models.BoardImage;
using LittleFlowerBot.Models.Message;
using Microsoft.Extensions.Configuration;

namespace LittleFlowerBot.Models.Renderer
{
    public class BufferedReplyRenderer : ITextRenderer
    {
        private readonly IMessage _message;
        private readonly string _baseUrl;
        private readonly List<ReplyMessageItem> _buffer = new();

        public string? ReplyToken { get; set; }
        public List<QuickReplyItem>? QuickReplyItems { get; set; }

        public BufferedReplyRenderer(IMessage message, IConfiguration configuration)
        {
            _message = message;
            _baseUrl = (configuration.GetValue<string>("BaseUrl") ?? "").TrimEnd('/');
        }

        public void Render(string text)
        {
            _buffer.Add(new TextReplyMessage(text));
        }

        public void RenderImage(byte[] boardState)
        {
            var encoded = BoardStateEncoder.Base64UrlEncode(boardState);
            var imageUrl = $"{_baseUrl}/api/board-images/{encoded}";
            _buffer.Add(new ImageReplyMessage(imageUrl));
        }

        public void Flush()
        {
            if (_buffer.Count == 0 || string.IsNullOrEmpty(ReplyToken))
            {
                return;
            }

            var messages = MergeConsecutiveTextMessages(_buffer);
            _message.ReplyMessages(ReplyToken, messages, QuickReplyItems);
            _buffer.Clear();
            QuickReplyItems = null;
        }

        private static List<ReplyMessageItem> MergeConsecutiveTextMessages(List<ReplyMessageItem> items)
        {
            var result = new List<ReplyMessageItem>();
            var textParts = new List<string>();

            foreach (var item in items)
            {
                if (item is TextReplyMessage textMsg)
                {
                    textParts.Add(textMsg.Text);
                }
                else
                {
                    if (textParts.Count > 0)
                    {
                        result.Add(new TextReplyMessage(string.Join("\n", textParts)));
                        textParts.Clear();
                    }
                    result.Add(item);
                }
            }

            if (textParts.Count > 0)
            {
                result.Add(new TextReplyMessage(string.Join("\n", textParts)));
            }

            return result;
        }
    }
}
