using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LittleFlowerBotTests.Models.Game.ChessGames.ChineseChess.StepRule
{
    public class StepRuleTests
    {
        protected LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess[][] Board;
        protected Step Step;
        protected IStepRule StepRule;

        public virtual void Setup()
        {
            Board = new LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess[10][];
            for (int i = 0; i < 10; i++)
            {
                Board[i] = new LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.ChineseChess[9];
                for (int j = 0; j < 9; j++)
                {
                    Board[i][j] = default;
                }
            }
        }

        protected void GivenStep(Step step)
        {
            Step = step;
        }

        protected void StepShouldNotMatch()
        {
            var isMatch = StepRule.IsMatch(Board, Step);

            ClassicAssert.IsFalse(isMatch);
        }
        
        protected void StepShouldMatch()
        {
            var isMatch = StepRule.IsMatch(Board, Step);

            ClassicAssert.IsTrue(isMatch);
        }
    }
}