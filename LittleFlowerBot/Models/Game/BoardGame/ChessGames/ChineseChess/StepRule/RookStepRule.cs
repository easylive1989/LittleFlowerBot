using System;
using System.Linq;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class RookStepRule : IStepRule
    {
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (!IsMoveStraight(step))
            {
                return false;
            }

            if (HasChessBlock(board, step))
            {
                return false;
            }
            
            return true;
        }
        
        private static bool IsMoveStraight(Step step)
        {
            return step.FromX == step.ToX || step.FromY == step.ToY;
        }
        
        private static bool HasChessBlock(ChineseChess[][] board, Step step)
        {
            if (step.FromX == step.ToX)
            {
                return Enumerable.Range(Math.Min(step.FromY, step.ToY) + 1, Math.Abs(step.ToY - step.FromY) - 1)
                    .Any(x => board[step.FromX][x] != ChineseChess.Empty);
            }
            else
            {
                return Enumerable.Range(Math.Min(step.FromX, step.ToX) + 1, Math.Abs(step.ToX - step.FromX) - 1)
                    .Any(x => board[x][step.FromY] != ChineseChess.Empty);
            }
        }
    }
}