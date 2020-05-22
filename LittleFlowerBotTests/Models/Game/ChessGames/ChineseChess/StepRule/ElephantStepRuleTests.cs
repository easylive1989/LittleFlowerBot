using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;
using static LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class ElephantStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new ElephantStepRule();
        }
        
        [Test]
        public void MoveElephantWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 2,
                ToX = 1,
                ToY = 3
            });

            StepShouldNotMatch();
        }
        
        [Test]
        public void MoveElephantWhenEyeStuck()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 2,
                ToX = 2,
                ToY = 4
            });

            Board[1][3] = Horse;

            StepShouldNotMatch();
        }
        
        [Test]
        public void MoveElephantCrossRiver()
        {
            GivenStep(new Step()
            {
                FromX = 4,
                FromY = 2,
                ToX = 6,
                ToY = 4
            });

            Board[4][2] = Elephant;

            StepShouldNotMatch();
        }
    }
}