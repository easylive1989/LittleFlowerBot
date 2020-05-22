using System;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class PawnStepRule : IStepRule
    {
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (!IsOneStep(step))
            {
                return false;
            }

            if (IsGoBack(board, step))
            {
                return false;
            }

            if (!IsInCrossSide(board, step) && IsGoRightOrLeft(step))
            {
                return false;
            }

            return true;
        }

        private bool IsGoRightOrLeft(Step step)
        {
            return step.ToY != step.FromY;
        }

        private bool IsInCrossSide(ChineseChess[][] board, Step step)
        {
            var chess = board[step.FromX][step.FromY];
            return (chess == ChineseChess.Soldier && step.FromX >= 5) ||
                   (chess == ChineseChess.Pawn && step.FromX <= 4);
        }

        private bool IsGoBack(ChineseChess[][] board, Step step)
        {
            var chess = board[step.FromX][step.FromY];
            return (chess == ChineseChess.Soldier && step.FromX > step.ToX) ||
                   (chess == ChineseChess.Pawn && step.ToX > step.FromX);
        }

        private static bool IsOneStep(Step step)
        {
            return Math.Abs(step.ToX - step.FromX) + Math.Abs(step.ToY - step.FromY) == 1;
        }
    }
}