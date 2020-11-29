using System.Collections.Generic;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Game;

namespace LittleFlowerBot.Models.Caches
{
    public interface IGameBoardCache
    {
        Task Set(string gameId, IGameBoard gameBoard);
        Task<IGameBoard> Get(string gameId);
        Task Remove(string gameId);
        List<string> GetGameIdList();
    }
}