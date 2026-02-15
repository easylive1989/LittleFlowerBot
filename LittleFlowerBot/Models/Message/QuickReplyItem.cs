namespace LittleFlowerBot.Models.Message
{
    public class QuickReplyItem
    {
        public string Label { get; set; }
        public string Text { get; set; }

        public QuickReplyItem(string label, string text)
        {
            Label = label;
            Text = text;
        }
    }
}
