namespace LittleFlowerBot.Models.BoardImage
{
    public interface IBoardImageStore
    {
        string Save(byte[] imageData);
        byte[]? Get(string imageId);
    }
}
