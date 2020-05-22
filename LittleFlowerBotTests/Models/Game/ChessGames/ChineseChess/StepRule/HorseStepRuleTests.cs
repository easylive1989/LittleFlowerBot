using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class HorseStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new HorseStepRule();
        }
        
        [Test]
        public void MoveHorseWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 1,
                ToX = 1,
                ToY = 2
            });

            StepShouldNotMatch();
        }
    }
}