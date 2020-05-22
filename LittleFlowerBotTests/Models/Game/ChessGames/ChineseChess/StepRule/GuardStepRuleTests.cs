using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class GuardStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new GuardStepRule();
        }
        
        [Test]
        public void MoveGuardWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 3,
                ToX = 1,
                ToY = 3
            });

            StepShouldNotMatch();
        }
        
        [Test]
        public void MoveGuardOutPalace()
        {
            GivenStep(new Step()
            {
                FromX = 9,
                FromY = 3,
                ToX = 8,
                ToY = 2
            });

            StepShouldNotMatch();
        }
        
    }
}