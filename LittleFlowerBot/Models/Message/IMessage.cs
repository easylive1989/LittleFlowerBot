namespace LittleFlowerBot.Models.Message
{
    public interface IMessage
    {
        void Reply(string replyToken, string text);
    }
}