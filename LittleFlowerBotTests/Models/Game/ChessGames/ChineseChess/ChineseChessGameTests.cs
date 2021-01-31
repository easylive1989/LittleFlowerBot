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
            
            _chineseChessGame.Act(PlayerA, "4,a>5,a");
            
            MessageShouldBe(
                "00ⒶⒷⒸⒹⒺⒻⒼⒽⒾ\n" +
                "01車馬象士將士象馬車\n" +
                "02┼┼┼┼┼┼┼┼┼\n" +
                "03┼包┼┼┼┼┼包┼\n" +
                "04┼┼卒┼卒┼卒┼卒\n" +
                "05卒┴┴┴┴┴┴┴┴\n" +
                "06┬┬┬┬┬┬┬┬┬\n" +
                "07兵┼兵┼兵┼兵┼兵\n" +
                "08┼炮┼┼┼┼┼炮┼\n" +
                "09┼┼┼┼┼┼┼┼┼\n" +
                "10俥傌相仕帥仕相傌俥\n");
        }
        
        [Test]
        public void Act_MoveSoldierUpAtFirstMove_ShouldShowNotYourTurn()
        {
            GivenTwoPlayerJoin();
            
            _chineseChessGame.Act(PlayerB, "7,a>6,a");
            
            MessageShouldBe("不是你的回合");
        }
        
        [Test]
        public void Act_MoveSoldierUpWithWrongPlayer_ShouldShowNotYourTurn()
        {
            GivenTwoPlayerJoin();
            
            _chineseChessGame.Act(PlayerA, "7,a>6,a");
            
            MessageShouldBe("不是你的棋子");
        }
        
        [Test]
        public void Act_MoveChessToInvalidCoordinate()
        {
            GivenTwoPlayerJoin();
            
            _chineseChessGame.Act(PlayerA, "1,a>4,a");
            
            MessageShouldBe("座標不合法");
        }

        [Test]
        public void Act_MoveChessToChess_ShouldAteChess()
        {
            GivenTwoPlayerJoin();
            
            _chineseChessGame.Act(PlayerA, "4,a>5,a");
            _chineseChessGame.Act(PlayerB, "7,a>6,a");
            _chineseChessGame.Act(PlayerA, "5,a>6,a");
            
            MessageShouldBe(
                "00ⒶⒷⒸⒹⒺⒻⒼⒽⒾ\n" +
                "01車馬象士將士象馬車\n" +
                "02┼┼┼┼┼┼┼┼┼\n" +
                "03┼包┼┼┼┼┼包┼\n" +
                "04┼┼卒┼卒┼卒┼卒\n" +
                "05┴┴┴┴┴┴┴┴┴\n" +
                "06卒┬┬┬┬┬┬┬┬\n" +
                "07┼┼兵┼兵┼兵┼兵\n" +
                "08┼炮┼┼┼┼┼炮┼\n" +
                "09┼┼┼┼┼┼┼┼┼\n" +
                "10俥傌相仕帥仕相傌俥\n");
        }
        
        [Test]
        public void IsGameOver_KingAte_ShouldBeTrue()
        {
            GivenTwoPlayerJoin();
            
            _chineseChessGame.Act(PlayerA, "4,e>5,e");
            _chineseChessGame.Act(PlayerB, "7,e>6,e");
            _chineseChessGame.Act(PlayerA, "5,e>6,e");
            _chineseChessGame.Act(PlayerB, "7,a>6,a");
            _chineseChessGame.Act(PlayerA, "6,e>6,d");
            _chineseChessGame.Act(PlayerB, "10,e>1,e");
            
            MessageShouldBe("遊戲結束!");
        }

        [Test]
        public void Act_MovePawnWithWrongWay()
        {
            GivenTwoPlayerJoin();
            
            _chineseChessGame.Act(PlayerA, "4,a>6,a");
            
            MessageShouldBe("走法有誤");
        }
        
        private void MessageShouldBe(string message)
        {
            _textRenderer.Received(1).Render(message);
        }
        
        private void GivenTwoPlayerJoin()
        {
            _chineseChessGame.Act(PlayerA, "++");
            _chineseChessGame.Act(PlayerB, "++");
        }

    }
}