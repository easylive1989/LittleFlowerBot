using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LittleFlowerBotTests.Models.Game.BoardGame.Gomoku
{
    [TestFixture]
    public class GomokuGameTests
    {
        private ITextRenderer _renderer;
        private GomokuGame _game;

        [Test]
        public void GameOver_Win()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "1a");
            InputCmd("userB", "2a");
            InputCmd("userA", "1b");
            InputCmd("userB", "2b");
            InputCmd("userA", "1c");
            InputCmd("userB", "2c");
            InputCmd("userA", "1d");
            InputCmd("userB", "2d");
            InputCmd("userA", "1e");

            MessageShouldBe("遊戲結束!");
        }

        [Test]
        public void Act_JoinTwoPlayer_ShouldShowStartMessage()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            
            MessageShouldBe("遊戲開始");
        }
        
        [Test]
        public void Act_JoinOnePlayer_ShouldBeNotGameOver()
        {
            StartGame();

            InputCmd("userA", "++");

            var isGameOver = _game.GameBoard.IsGameOver();
            
            ClassicAssert.IsFalse(isGameOver);
        }


        private void MessageShouldBe(string message)
        {
            _renderer.Received(1).Render(message);
        }

        private void StartGame()
        {
            _renderer = Substitute.For<ITextRenderer>();
            var dualGameResultsRepository = Substitute.For<IBoardGameResultsRepository>();
            _game = new GomokuGame(dualGameResultsRepository);
            _game.TextRenderer = _renderer;

            _game.StartGame();
        }

        private void InputCmd(string userId, string cmd)
        {
            _game.Act(userId, cmd);
        }
    }
}