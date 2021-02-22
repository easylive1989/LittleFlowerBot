using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
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
        private IDistributedCache _distributedCache;
        private GameBoardCache _gameBoardCache;

        [SetUp]
        public void SetUp()
        {
            _distributedCache = Substitute.For<IDistributedCache>();
            _gameBoardCache = new GameBoardCache(_distributedCache);
        }

        [Test]
        public async Task StoreJsonToCache()
        {
            await _gameBoardCache.Set("gameId", new TicTacToeBoard());

            await ShouldStoreSomethingWithKey("gameId:type");
            await ShouldStoreSomethingWithKey("gameId:state");
        }

        [Test]
        public async Task GetGameFromJson()
        {
            var gameTypeBytes = Encoding.UTF8.GetBytes("1");
            var gameStateBytes = Encoding.UTF8.GetBytes("{\"CurMoveX\":0,\"CurMoveY\":0,\"CurPlayer\":null,\"Row\":3,\"Column\":3,\"GameBoardArray\":[[0,0,0],[0,0,0],[0,0,0]],\"PlayerMap\":{\"1\":null,\"2\":null},\"PlayerMoveOrder\":[]}");
            _distributedCache.GetAsync("gameId:state").Returns(gameStateBytes);
            _distributedCache.GetAsync("gameId:type").Returns(gameTypeBytes);
            
            var gameBoard = await _gameBoardCache.Get("gameId");
            
            new TicTacToeBoard().ToExpectedObject().ShouldEqual(gameBoard);
        }

        private async Task ShouldStoreSomethingWithKey(string key)
        {
            await _distributedCache.Received(1)
                .SetAsync(key, Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }
    }
}