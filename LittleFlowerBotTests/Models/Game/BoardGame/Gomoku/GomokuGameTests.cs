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
            InputCmd("userA", "1,a");
            InputCmd("userB", "2,a");
            InputCmd("userA", "1,b");
            InputCmd("userB", "2,b");
            InputCmd("userA", "1,c");
            InputCmd("userB", "2,c");
            InputCmd("userA", "1,d");
            InputCmd("userB", "2,d");
            InputCmd("userA", "1,e");

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