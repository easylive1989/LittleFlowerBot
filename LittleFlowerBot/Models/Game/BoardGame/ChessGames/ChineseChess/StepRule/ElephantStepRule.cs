using System;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class ElephantStepRule : IStepRule
    {
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (!IsFarmStep(step))
            {
                return false;
            }

            if (IsCrossRiver(board, step))
            {
                return false;
            }
            
            if (IsElephantEyeStuck(board, step))
            {
                return false;
            }

            return true;
        }

        private bool IsCrossRiver(ChineseChess[][] board, Step step)
        {
            var chess = board[step.FromX][step.FromY];
            return (chess == ChineseChess.Elephant && step.ToX >= 5) ||
                   (chess == ChineseChess.Minister && step.ToX <= 4);
        }

        private bool IsElephantEyeStuck(ChineseChess[][] board, Step step)
        {
            var x = (step.FromX + step.ToX) / 2;
            var y = (step.FromY + step.ToY) / 2;
            return board[x][y] != ChineseChess.Empty;
        }

        private static bool IsFarmStep(Step step)
        {
            return Math.Abs(step.ToX - step.FromX) == 2 && Math.Abs(step.ToY - step.FromY) == 2;
        }
    }
}