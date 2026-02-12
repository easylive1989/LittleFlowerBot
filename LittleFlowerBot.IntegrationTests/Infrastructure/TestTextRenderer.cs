using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

/// <summary>
/// 測試用的 TextRenderer 替身
/// 記錄所有 Render/Reply 的訊息，供測試驗證使用
/// </summary>
public class TestTextRenderer : ITextRenderer, IMessage, ILineNotifySubscription
{
    private static readonly List<string> _messages = new();

    public void Render(string text) => _messages.Add(text);

    public void Reply(string replyToken, string text) => _messages.Add(text);

    public Task SaveToken(string senderId, string code) => Task.CompletedTask;

    public string GenerateLink(string guid) => guid;

    public bool IsRegistered(string senderId) => false;

    /// <summary>
    /// 取得所有已記錄的訊息
    /// </summary>
    public static IReadOnlyList<string> Messages => _messages;

    /// <summary>
    /// 清除所有已記錄的訊息（每個測試場景前呼叫）
    /// </summary>
    public static void Clear() => _messages.Clear();
}
