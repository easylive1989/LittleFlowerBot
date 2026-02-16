using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

/// <summary>
/// 測試用的 TextRenderer 替身
/// 記錄所有 Render/Reply 的訊息，供測試驗證使用
/// </summary>
public class TestTextRenderer : ITextRenderer, IMessage
{
    private static readonly List<string> _messages = new();

    public void Render(string text) => _messages.Add(text);

    public void RenderImage(byte[] imageData) => _messages.Add("[BoardImage]");

    public void Reply(string replyToken, string text, List<QuickReplyItem>? quickReplyItems = null) => _messages.Add(text);

    public void ReplyMessages(string replyToken, List<ReplyMessageItem> messages, List<QuickReplyItem>? quickReplyItems = null)
    {
        foreach (var msg in messages)
        {
            switch (msg)
            {
                case TextReplyMessage textMsg:
                    _messages.Add(textMsg.Text);
                    break;
                case ImageReplyMessage imageMsg:
                    _messages.Add("[BoardImage]");
                    break;
            }
        }
    }

    /// <summary>
    /// 取得所有已記錄的訊息
    /// </summary>
    public static IReadOnlyList<string> Messages => _messages;

    /// <summary>
    /// 清除所有已記錄的訊息（每個測試場景前呼叫）
    /// </summary>
    public static void Clear() => _messages.Clear();
}
