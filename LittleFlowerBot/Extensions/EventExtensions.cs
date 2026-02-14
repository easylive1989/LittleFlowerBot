using isRock.LineBot;

namespace LittleFlowerBot.Extensions
{
    public static class EventExtensions
    {
        public static string SenderId(this Event @event)
        {
            var source = @event.source;
            return source.groupId ?? (source.roomId ?? source.userId);
        }

        public static string UserId(this Event @event)
        {
            return @event.source.userId;
        }

        public static string? Text(this Event @event)
        {
            return @event.message?.text;
        }
    }
}