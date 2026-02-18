using LittleFlowerBot.Models.Game.BoardGame;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess
{
    [TestFixture]
    public class ChineseChessGameTests
    {
        private ChineseChessGame _chineseChessGame;
        private ITextRenderer _textRenderer;
        private readonly string PlayerA = "1234";
        private readonly string PlayerB = "4567";

        [SetUp]
        public void Setup()
        {
            _textRenderer = Substitute.For<ITextRenderer>();
            _chineseChessGame = new ChineseChessGame();
            _chineseChessGame.TextRenderer = _textRenderer;
        }

        [Test]
        public void Act_JoinSamePlayer_ShouldShowDuplicatedJoinMessge()
        {
            _chineseChessGame.Act(PlayerA, "++");
            _chineseChessGame.Act(PlayerA, "++");
            
            MessageShouldBe("你已經加入");
        }
        
        [Test]
        public void Act_JoinTwoPlayer_ShouldShowStartMessage()
        {
            GivenTwoPlayerJoin();

            MessageShouldBe("遊戲開始");
        }

        [Test]
        public void Act_MoveSoldierUp_ShouldShowGameBoard()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerA, "4a>5a");

            ImageShouldBeRendered(2);
        }

        [Test]
        public void Act_MoveSoldierUpAtFirstMove_ShouldShowNotYourTurn()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerB, "7a>6a");

            MessageShouldBe("不是你的回合");
        }

        [Test]
        public void Act_MoveSoldierUpWithWrongPlayer_ShouldShowNotYourTurn()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerA, "7a>6a");

            MessageShouldBe("不是你的棋子");
        }

        [Test]
        public void Act_MoveChessToInvalidCoordinate()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerA, "1a>4a");

            MessageShouldBe("座標不合法");
        }

        [Test]
        public void Act_MoveChessToChess_ShouldAteChess()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerA, "4a>5a");
            _chineseChessGame.Act(PlayerB, "7a>6a");
            _chineseChessGame.Act(PlayerA, "5a>6a");

            ImageShouldBeRendered(4);
        }

        [Test]
        public void IsGameOver_KingAte_ShouldBeTrue()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerA, "4e>5e");
            _chineseChessGame.Act(PlayerB, "7e>6e");
            _chineseChessGame.Act(PlayerA, "5e>6e");
            _chineseChessGame.Act(PlayerB, "7a>6a");
            _chineseChessGame.Act(PlayerA, "6e>6d");
            _chineseChessGame.Act(PlayerB, "10e>1e");

            MessageShouldBe("遊戲結束!");
        }

        [Test]
        public void Act_MovePawnWithWrongWay()
        {
            GivenTwoPlayerJoin();

            _chineseChessGame.Act(PlayerA, "4a>6a");
            
            MessageShouldBe("走法有誤");
        }
        
        private void MessageShouldBe(string message)
        {
            _textRenderer.Received(1).Render(message);
        }

        private void ImageShouldBeRendered(int times)
        {
            _textRenderer.Received(times).RenderImage(Arg.Any<byte[]>());
        }
        
        private void GivenTwoPlayerJoin()
        {
            _chineseChessGame.Act(PlayerA, "++");
            _chineseChessGame.Act(PlayerB, "++");
        }

    }
}