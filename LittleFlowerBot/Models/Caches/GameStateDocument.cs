using MongoDB.Bson.Serialization.Attributes;

namespace LittleFlowerBot.Models.Caches
{
    public class GameStateDocument
    {
        [BsonId]
        public string Id { get; set; }

        public int GameType { get; set; }

        public string State { get; set; }
    }
}
