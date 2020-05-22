using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;
using static LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class RookStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new RookStepRule();
        }
        
        [Test]
        public void MoveRookWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 0,
                ToX = 1,
                ToY = 1
            });

            StepShouldNotMatch();
        }
        
        [Test]
        public void Rook_Move_Cross_Chess()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 0,
                ToX = 3,
                ToY = 0
            });

            Board[0][0] = Rook;
            Board[2][0] = Pawn;
            
            StepShouldNotMatch();
        }
    }
}