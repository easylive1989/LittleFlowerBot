using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;
using static LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class GeneralStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new GeneralStepRule();
        }

        [Test]
        public void MoveGeneralWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 4,
                ToX = 1,
                ToY = 3
            });

            StepShouldNotMatch();
        }

        [Test]
        public void MoveGeneralWhenKingToKing()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 4,
                ToX = 9,
                ToY = 4
            });

            Board[0][4] = General;
            Board[9][4] = King;
            
            StepShouldMatch();
        }

        [Test]
        public void MoveKingOutPalace()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 3,
                ToX = 0,
                ToY = 2
            });
            
            StepShouldNotMatch();
        }
    }
}