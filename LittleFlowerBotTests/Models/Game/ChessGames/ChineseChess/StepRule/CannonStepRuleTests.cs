using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;
using static LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    [TestFixture]
    public class CannonStepRuleTests : StepRuleTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            StepRule = new CannonStepRule();
        }
        
        [Test]
        public void MoveCannonWithWrongWay()
        {
            GivenStep(new Step()
            {
                FromX = 2,
                FromY = 1,
                ToX = 3,
                ToY = 2
            });
            
            StepShouldNotMatch();
        }
        
        [Test]
        public void Cannon_Move_Cross_Chess()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 0,
                ToX = 3,
                ToY = 0
            });

            Board[0][0] = Cannon;
            Board[2][0] = Pawn;
            
            StepShouldNotMatch();
        }
        
        [Test]
        public void Cannon_Eat_Without_Hop()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 0,
                ToX = 3,
                ToY = 0
            });

            Board[0][0] = Cannon;
            Board[3][0] = Pawn;
            
            StepShouldNotMatch();
        }
        
        [Test]
        public void Cannon_Eat_With_Hop()
        {
            GivenStep(new Step()
            {
                FromX = 0,
                FromY = 0,
                ToX = 3,
                ToY = 0
            });

            Board[0][0] = Cannon;
            Board[1][0] = Soldier;
            Board[3][0] = Pawn;
            
            StepShouldMatch();
        }
    }
}