namespace LittleFlowerBot.Models.Renderer
{
    public interface ITextRenderer
    {
        void Render(string text);
        void RenderImage(byte[] imageData);
    }
}