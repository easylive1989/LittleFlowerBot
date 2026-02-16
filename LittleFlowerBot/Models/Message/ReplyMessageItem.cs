namespace LittleFlowerBot.Models.Message
{
    public abstract class ReplyMessageItem
    {
    }

    public class TextReplyMessage : ReplyMessageItem
    {
        public string Text { get; }

        public TextReplyMessage(string text)
        {
            Text = text;
        }
    }

    public class ImageReplyMessage : ReplyMessageItem
    {
        public string ImageUrl { get; }

        public ImageReplyMessage(string imageUrl)
        {
            ImageUrl = imageUrl;
        }
    }
}
