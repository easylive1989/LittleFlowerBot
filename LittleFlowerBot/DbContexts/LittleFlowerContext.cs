using LittleFlowerBot.Models.GameResult;
using MongoDB.Driver;

namespace LittleFlowerBot.DbContexts
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<BoardGameResult> BoardGameResults =>
            _database.GetCollection<BoardGameResult>("BoardGameResults");
    }
}
