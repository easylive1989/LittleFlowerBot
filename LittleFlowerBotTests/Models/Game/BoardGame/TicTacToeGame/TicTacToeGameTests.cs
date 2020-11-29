using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.Game.BoardGame.KiGames;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.BoardGame.TicTacToeGame
{
    [TestFixture]
    public class TicTacToeGameTests
    {
        private ITextRenderer _renderer;
        private LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe.TicTacToeGame _game;

        [Test]
        public void StartGame_ShowInviteMessage()
        {
            StartGame();

            MessageShouldBe("輸入++參加遊戲");
        }

        [Test]
        public void Join2Player_ShowGameStartMessage()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");

            MessageShouldBe("遊戲開始");
        }

        [Test]
        public void IsMatchForJoinPlayer_Success()
        {
            StartGame();

            bool isMatch = _game.IsMatch("++");

            Assert.IsTrue(isMatch);
        }

        [Test]
        public void IsMatchForJoinPlayer_Fail()
        {
            StartGame();

            bool isMatch = _game.IsMatch("shouldNotMatch");

            Assert.IsFalse(isMatch);
        }

        [Test]
        public void IsMatchForPlaying_Success()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");

            bool isMatch = _game.IsMatch("1,c");

            Assert.IsTrue(isMatch);
        }

        [Test]
        public void IsMatchForPlaying_Fail()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");

            bool isMatch = _game.IsMatch("12,c");

            Assert.IsFalse(isMatch);
        }

        [Test]
        public void JoinSamePlayer_ShowAlreadyJoinMessage()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userA", "++");

            MessageShouldBe("你已經加入");
        }

        [Test]
        public void PlayGame_PlaceChess()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "2,b");

            MessageShouldBe("00ⒶⒷⒸ\n01＿＿＿\n02＿●＿\n03＿＿＿\n");
        }

        [Test]
        public void PlayGame_PlaceTwoChess()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "2,b");
            InputCmd("userB", "1,a");

            MessageShouldBe("00ⒶⒷⒸ\n01○＿＿\n02＿●＿\n03＿＿＿\n");
        }

        [Test]
        public void GameOver_Win()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "2,b");
            InputCmd("userB", "1,a");
            InputCmd("userA", "2,a");
            InputCmd("userB", "1,b");
            InputCmd("userA", "2,c");

            MessageShouldBe("遊戲結束!");
        }

        [Test]
        public void GameOver_Draw()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "2,b");
            InputCmd("userB", "1,a");
            InputCmd("userA", "1,b");
            InputCmd("userB", "3,b");
            InputCmd("userA", "2,a");
            InputCmd("userB", "2,c");
            InputCmd("userA", "3,a");
            InputCmd("userB", "1,c");
            InputCmd("userA", "3,c");

            MessageShouldBe("遊戲結束!");
        }

        [Test]
        public void GameOver_WinInAntiSlash()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "1,c");
            InputCmd("userB", "1,a");
            InputCmd("userA", "2,b");
            InputCmd("userB", "2,a");
            InputCmd("userA", "3,a");

            MessageShouldBe("遊戲結束!");
        }

        [Test]
        public void PlayGame_NotYourTurn()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userB", "2,2");

            MessageShouldBe("不是你的回合");
        }

        [Test]
        public void PlayGame_CoordinateNotValid()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "2,b");
            InputCmd("userB", "2,b");

            MessageShouldBe("座標不合法");
        }

        private void MessageShouldBe(string message)
        {
            _renderer.Received(1).Render(message);
        }

        private void StartGame()
        {
            _renderer = Substitute.For<ITextRenderer>();
            var dualGameResultsRepository = Substitute.For<IBoardGameResultsRepository>();
            _game = new LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe.TicTacToeGame(dualGameResultsRepository);
            _game.TextRenderer = _renderer;
            
            _game.StartGame();
        }

        private void InputCmd(string userId, string cmd)
        {
            _game.Act(userId, cmd);
        }
    }
}