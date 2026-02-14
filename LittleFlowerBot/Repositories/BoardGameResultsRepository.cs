using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.GameResult;
using MongoDB.Driver;

namespace LittleFlowerBot.Repositories
{
    public class BoardGameResultsRepository : IBoardGameResultsRepository
    {
        private readonly MongoDbContext _context;

        public BoardGameResultsRepository(MongoDbContext context)
        {
            _context = context;
        }

        public void AddGameResult(BoardGameResult boardGameResult)
        {
            _context.BoardGameResults.InsertOne(boardGameResult);
        }

        public IEnumerable<BoardGameResult> GetAll()
        {
            return _context.BoardGameResults.Find(_ => true).ToList();
        }

        public async Task<IEnumerable<BoardGameResult>> GetResult(string userId)
        {
            return await _context.BoardGameResults
                .Find(x => x.UserId == userId)
                .ToListAsync();
        }
    }

    public interface IBoardGameResultsRepository
    {
        void AddGameResult(BoardGameResult boardGameResult);
        IEnumerable<BoardGameResult> GetAll();
        Task<IEnumerable<BoardGameResult>> GetResult(string userId);
    }
}
