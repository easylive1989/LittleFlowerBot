using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;
using static LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class PawnStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new PawnStepRule();
        }

        [Test]
        public void MovePawnWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 3,
                FromY = 0,
                ToX = 4,
                ToY = 1
            });

            StepShouldNotMatch();
        }
        
        [Test]
        public void Soldier_Go_Back()
        {
            GivenStep(new Step()
            {
                FromX = 3,
                FromY = 0,
                ToX = 2,
                ToY = 0
            });

            Board[3][0] = Soldier;

            StepShouldNotMatch();
        }
        
        [Test]
        public void Soldier_Go_Right_Before_Cross_River()
        {
            GivenStep(new Step()
            {
                FromX = 3,
                FromY = 0,
                ToX = 3,
                ToY = 1
            });

            Board[3][0] = Soldier;

            StepShouldNotMatch();
        }
    }
}