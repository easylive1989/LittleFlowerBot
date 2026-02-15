using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpectedObjects;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Cache
{
    [TestFixture]
    public class GameBoardCacheTests
    {
        private IMongoCollection<GameStateDocument> _collection;
        private GameBoardCache _gameBoardCache;

        [SetUp]
        public void SetUp()
        {
            var database = Substitute.For<IMongoDatabase>();
            _collection = Substitute.For<IMongoCollection<GameStateDocument>>();
            database.GetCollection<GameStateDocument>("GameStates").Returns(_collection);
            var context = new MongoDbContext(database);
            _gameBoardCache = new GameBoardCache(context);
        }

        [Test]
        public async Task StoreJsonToCache()
        {
            await _gameBoardCache.Set("gameId", new TicTacToeBoard());

            await _collection.Received(1).ReplaceOneAsync(
                Arg.Any<FilterDefinition<GameStateDocument>>(),
                Arg.Is<GameStateDocument>(d => d.Id == "gameId" && d.GameType == (int)GameType.TicTacToe),
                Arg.Any<ReplaceOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task GetGameFromJson()
        {
            var state = "{\"CurMoveX\":0,\"CurMoveY\":0,\"CurPlayer\":null,\"Row\":3,\"Column\":3,\"GameBoardArray\":[[0,0,0],[0,0,0],[0,0,0]],\"PlayerMap\":{\"1\":null,\"2\":null},\"PlayerMoveOrder\":[]}";
            var document = new GameStateDocument
            {
                Id = "gameId",
                GameType = (int)GameType.TicTacToe,
                State = state
            };

            var mockCursor = Substitute.For<IAsyncCursor<GameStateDocument>>();
            mockCursor.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(true, false);
            mockCursor.Current.Returns(new List<GameStateDocument> { document });

            _collection.FindAsync(
                Arg.Any<FilterDefinition<GameStateDocument>>(),
                Arg.Any<FindOptions<GameStateDocument, GameStateDocument>>(),
                Arg.Any<CancellationToken>())
                .Returns(mockCursor);

            var gameBoard = await _gameBoardCache.Get("gameId");

            new TicTacToeBoard().ToExpectedObject().ShouldEqual(gameBoard);
        }
    }
}
