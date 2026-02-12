namespace LittleFlowerBot.Models.Renderer
{
    public interface IRendererFactory
    {
        ITextRenderer Get(string replyToken);
    }
}