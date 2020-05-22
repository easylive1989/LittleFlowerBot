using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.GameResult;
using Microsoft.EntityFrameworkCore;

namespace LittleFlowerBot.Repositories
{
    public class BoardGameResultsRepository : IBoardGameResultsRepository
    {
        private readonly LittleFlowerBotContext _littleFlowerBotContext;

        public BoardGameResultsRepository(LittleFlowerBotContext littleFlowerBotContext)
        {
            _littleFlowerBotContext = littleFlowerBotContext;
        }

        public void AddGameResult(BoardGameResult boardGameResult)
        {
            _littleFlowerBotContext.BoardGameGameResults.Add(boardGameResult);
            _littleFlowerBotContext.SaveChanges();
        }

        public IEnumerable<BoardGameResult> GetAll()
        {
            return _littleFlowerBotContext.BoardGameGameResults.ToList();
        }

        public async Task<IEnumerable<BoardGameResult>> GetResult(string userId)
        {
            return await _littleFlowerBotContext.BoardGameGameResults.Where(x => x.UserId.Equals(userId)).ToListAsync();
        }
    }

    public interface IBoardGameResultsRepository
    {
        void AddGameResult(BoardGameResult boardGameResult);
        IEnumerable<BoardGameResult> GetAll();
        Task<IEnumerable<BoardGameResult>> GetResult(string userId);
    }
}