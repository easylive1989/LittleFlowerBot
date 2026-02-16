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
        private LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe.TicTacToeGame _game;
        private ITextRenderer _renderer;

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

            ImageShouldBeRendered(2);
        }

        [Test]
        public void PlayGame_PlaceTwoChess()
        {
            StartGame();

            InputCmd("userA", "++");
            InputCmd("userB", "++");
            InputCmd("userA", "2,b");
            InputCmd("userB", "1,a");

            ImageShouldBeRendered(3);
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
            InputCmd("userB", "2,a");

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

        private void InputCmd(string userId, string cmd)
        {
            _game.Act(userId, cmd);
        }

        private void MessageShouldBe(string message)
        {
            _renderer.Received(1).Render(message);
        }

        private void ImageShouldBeRendered(int times)
        {
            _renderer.Received(times).RenderImage(Arg.Any<byte[]>());
        }

        private void StartGame()
        {
            _renderer = Substitute.For<ITextRenderer>();
            var dualGameResultsRepository = Substitute.For<IBoardGameResultsRepository>();
            _game = new LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe.TicTacToeGame(
                dualGameResultsRepository);
            _game.TextRenderer = _renderer;

            _game.StartGame();
        }
    }
}