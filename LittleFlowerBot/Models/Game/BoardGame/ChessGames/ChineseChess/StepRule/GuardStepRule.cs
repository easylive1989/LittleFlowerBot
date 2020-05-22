using System;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class GuardStepRule : IStepRule
    {
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (IsOutPalace(board, step))
            {
                return false;
            }

            return IsDiagonalStep(step);
        }

        private bool IsOutPalace(ChineseChess[][] board, Step step)
        {
            return step.ToY <= 2 || step.ToY >= 6 ||
                   (board[step.FromX][step.FromY] == ChineseChess.Guard && step.FromX >= 3) ||
                   (board[step.FromX][step.FromY] == ChineseChess.Adviser && step.FromX <= 6);
        }
        
        private static bool IsDiagonalStep(Step step)
        {
            return Math.Abs(step.ToX - step.FromX) == 1 && Math.Abs(step.ToY - step.FromY) == 1;
        }
    }
}