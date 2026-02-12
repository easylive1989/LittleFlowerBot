using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

/// <summary>
/// 測試用的 RendererFactory 替身
/// 始終返回 TestTextRenderer，不依賴 LINE API
/// </summary>
public class TestRendererFactory : IRendererFactory
{
    private readonly TestTextRenderer _renderer;

    public TestRendererFactory(TestTextRenderer renderer)
    {
        _renderer = renderer;
    }

    public ITextRenderer Get(string replyToken) => _renderer;
}
