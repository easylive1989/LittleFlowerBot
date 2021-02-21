using System.Threading.Tasks;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Cache
{
    [TestFixture]
    public class GameBoardCacheTests
    {
        [Test]
        public async Task StoreJsonToCache()
        {
            var distributedCache = Substitute.For<IDistributedCache>();
            var gameBoardCache = new GameBoardCache(distributedCache);

            await gameBoardCache.Set("gameId", new TicTacToeBoard());

            await distributedCache.Received(1).SetAsync("gameId:type", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
            await distributedCache.Received().SetAsync("gameId:state", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }
   }
}