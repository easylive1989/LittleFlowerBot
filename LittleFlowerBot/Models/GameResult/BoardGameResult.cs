using LittleFlowerBot.Models.Game;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LittleFlowerBot.Models.GameResult
{
    public class BoardGameResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public GameType GameType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public GameResult Result { get; set; }

        public DateTime GameOverTime { get; set; }
    }
}
