namespace LittleFlowerBot.Models.Renderer
{
    public interface ITextRenderer
    {
        void Render(string text);
        void RenderImage(byte[] imageData);
        void RenderPrivate(string userId, string text);
        void RenderPrivateImage(string userId, byte[] imageData);
    }
}