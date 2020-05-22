using System;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class HorseStepRule : IStepRule
    {
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (!IsOneSpaceOutwardDiagonalStep(step))
            {
                return false;
            }

            if (IsBlockSide(board, step))
            {
                return false;
            }

            return true;
        }

        private bool IsBlockSide(ChineseChess[][] board, Step step)
        {
            var diffX = Math.Abs(step.FromX - step.ToX);
            var diffY = Math.Abs(step.FromY - step.ToY);
            if (diffX > diffY)
            {
                if (step.FromX < step.ToX)
                {
                    return board[step.FromX + 1][step.FromY] != ChineseChess.Empty;
                }
                else
                {
                    return board[step.FromX - 1][step.FromY] != ChineseChess.Empty;
                }
            }
            else
            {
                if (step.FromY < step.ToY)
                {
                    return board[step.FromX][step.FromY + 1] != ChineseChess.Empty;
                }
                else
                {
                    return board[step.FromX][step.FromY - 1] != ChineseChess.Empty;
                }
            }
        }

        private static bool IsOneSpaceOutwardDiagonalStep(Step step)
        {
            return Math.Abs(step.ToX - step.FromX) + Math.Abs(step.ToY - step.FromY) == 3;
        }
    }
}