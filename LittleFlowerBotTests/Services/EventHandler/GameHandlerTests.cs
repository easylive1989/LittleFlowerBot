using System;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.Battleship;
using LittleFlowerBot.Models.Message;
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
        private IRendererFactory _rendererFactory;
        private ITextRenderer _renderer;
        private ILineUserService _lineUserService;
        private GameHandler _gameHandler;

        [SetUp]
        public void SetUp()
        {
            _gameFactory = Substitute.For<IGameFactory>();
            _gameBoardCache = Substitute.For<IGameBoardCache>();
            _rendererFactory = Substitute.For<IRendererFactory>();
            _renderer = Substitute.For<ITextRenderer>();
            _rendererFactory.Get(Arg.Any<string?>()).Returns(_renderer);
            _lineUserService = Substitute.For<ILineUserService>();
            _gameHandler = new GameHandler(_gameFactory, _gameBoardCache, _rendererFactory, _lineUserService);
        }

        [Test]
        public void GameOver_Should_Remove_Game()
        {
            GivenGameBoardExist();
            GivenRunningMockGame();
            GameAct("我認輸了");
            MockGameShouldReceiveGameOver();
        }

        [Test]
        public void JoinBattleship_WhenNotFollower_ShouldRejectAndNotCallAct()
        {
            GivenBattleshipBoardExist();
            GivenRunningMockGame();
            _lineUserService.IsFollower("userA").Returns(false);

            _gameHandler.Act("gameId", "userA", "++", "replyToken");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("好友")));
            _mockGame.DidNotReceive().Act(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void JoinBattleship_WhenIsFollower_ShouldProceedWithAct()
        {
            GivenBattleshipBoardExist();
            GivenRunningMockGame();
            _lineUserService.IsFollower("userA").Returns(true);

            _gameHandler.Act("gameId", "userA", "++", "replyToken");

            _mockGame.Received().Act("userA", "++");
        }

        [Test]
        public void JoinBattleship_WhenNoLineUserService_ShouldProceedWithAct()
        {
            var handlerWithoutService = new GameHandler(_gameFactory, _gameBoardCache, _rendererFactory, null);
            GivenBattleshipBoardExist();
            GivenRunningMockGame();

            handlerWithoutService.Act("gameId", "userA", "++", "replyToken");

            _mockGame.Received().Act("userA", "++");
        }

        [Test]
        public void JoinNonBattleship_WhenNotFollower_ShouldStillProceed()
        {
            GivenGameBoardExist();
            GivenRunningMockGame();
            _lineUserService.IsFollower("userA").Returns(false);

            _gameHandler.Act("gameId", "userA", "++", "replyToken");

            _mockGame.Received().Act("userA", "++");
        }

        private void GameAct(string action)
        {
            _gameHandler.Act("gameId", "userId", action, "replyToken");
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

        private void GivenBattleshipBoardExist()
        {
            var board = new BattleshipBoard();
            _gameBoardCache.Get(Arg.Any<string>()).Returns(board);
        }
    }
}
