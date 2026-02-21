using System.Collections.Generic;
using LittleFlowerBot.Models.BoardImage;
using LittleFlowerBot.Models.Message;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LittleFlowerBot.Models.Renderer
{
    public class BufferedReplyRenderer : ITextRenderer
    {
        private readonly IMessage _message;
        private readonly string _baseUrl;
        private readonly ILogger<BufferedReplyRenderer> _logger;
        private readonly List<ReplyMessageItem> _buffer = new();
        private readonly Dictionary<string, List<ReplyMessageItem>> _privateBuffers = new();

        public string? ReplyToken { get; set; }
        public List<QuickReplyItem>? QuickReplyItems { get; set; }

        public BufferedReplyRenderer(IMessage message, IConfiguration configuration, ILogger<BufferedReplyRenderer> logger)
        {
            _message = message;
            _baseUrl = (Environment.GetEnvironmentVariable("BaseUrl")
                ?? configuration.GetValue<string>("BaseUrl")
                ?? "").TrimEnd('/');
            _logger = logger;
        }

        public void Render(string text)
        {
            _buffer.Add(new TextReplyMessage(text));
        }

        public void RenderImage(byte[] boardState)
        {
            var encoded = BoardStateEncoder.Base64UrlEncode(boardState);
            var imageUrl = $"{_baseUrl}/api/board-images/{encoded}";
            _logger.LogInformation("Board image URL generated: {ImageUrl}", imageUrl);
            _buffer.Add(new ImageReplyMessage(imageUrl));
        }

        public void RenderPrivate(string userId, string text)
        {
            if (!_privateBuffers.ContainsKey(userId))
                _privateBuffers[userId] = new List<ReplyMessageItem>();
            _privateBuffers[userId].Add(new TextReplyMessage(text));
        }

        public void RenderPrivateImage(string userId, byte[] boardState)
        {
            if (!_privateBuffers.ContainsKey(userId))
                _privateBuffers[userId] = new List<ReplyMessageItem>();
            var encoded = BoardStateEncoder.Base64UrlEncode(boardState);
            var imageUrl = $"{_baseUrl}/api/board-images/{encoded}";
            _privateBuffers[userId].Add(new ImageReplyMessage(imageUrl));
        }

        public void Flush()
        {
            if (_buffer.Count > 0 && !string.IsNullOrEmpty(ReplyToken))
            {
                var messages = MergeConsecutiveTextMessages(_buffer);
                _message.ReplyMessages(ReplyToken, messages, QuickReplyItems);
                _buffer.Clear();
                QuickReplyItems = null;
            }

            foreach (var (userId, items) in _privateBuffers)
            {
                if (items.Count > 0)
                {
                    var messages = MergeConsecutiveTextMessages(items);
                    _message.PushMessages(userId, messages);
                }
            }
            _privateBuffers.Clear();
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
