using System;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Services.EventHandler;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Services.EventHandler
{
    [TestFixture]
    public class GameHandlerTests
    {
        private Game _mockGame;
        private IGameBoardCache _gameBoardCache;
        private IGameFactory _gameFactory;
        private GameHandler _gameHandler;

        [SetUp]
        public void SetUp()
        {
            _gameFactory = Substitute.For<IGameFactory>();
            _gameBoardCache = Substitute.For<IGameBoardCache>();
            var rendererFactory = Substitute.For<IRendererFactory>();
            _gameHandler = new GameHandler(_gameFactory, _gameBoardCache, rendererFactory);
        }

        [Test]
        public void GameOver_Should_Remove_Game()
        {
            GivenGameBoardExist();
            GivenRunningMockGame();
            GameAct("我認輸了");
            MockGameShouldReceiveGameOver();
        }

        private void GameAct(string action)
        {
            _gameHandler.Act("gameId", "userId", action);
        }

        private void MockGameShouldReceiveGameOver()
        {
            _mockGame.Received(1).GameOver();
        }

        private void GivenRunningMockGame()
        {
            _mockGame = Substitute.For<Game>();
            var mockBoard = Substitute.For<IGameBoard>();
            mockBoard.IsGameOver().Returns(false);
            _mockGame.GameBoard = mockBoard;
            _gameFactory.CreateGame(Arg.Any<IGameBoard>()).Returns(_mockGame);
        }

        private void GivenGameBoardExist()
        {
            _gameBoardCache.Get(Arg.Any<string>()).Returns(Substitute.For<IGameBoard>());
        }
    }
}