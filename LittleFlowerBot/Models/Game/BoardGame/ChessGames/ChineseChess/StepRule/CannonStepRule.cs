using System;
using System.Linq;
using LittleFlowerBot.Extensions;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class CannonStepRule : IStepRule
    {
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (!IsMoveStraight(step))
            {
                return false;
            }
            
            if (IsEat(board, step))
            {
                return HasHop(board, step);
            }

            if (HasChessBlock(board, step))
            {
                return false;
            }
            
            return true;
        }

        private bool HasHop(ChineseChess[][] board, Step step)
        {
            if (step.FromX == step.ToX)
            {
                return Enumerable.Range(Math.Min(step.FromY, step.ToY) + 1, Math.Abs(step.ToY - step.FromY) - 1)
                    .Count(x => board[step.FromX][x] != ChineseChess.Empty) == 1;
            }
            else
            {
                return Enumerable.Range(Math.Min(step.FromX, step.ToX) + 1, Math.Abs(step.ToX - step.FromX) - 1)
                    .Count(x => board[x][step.FromY] != ChineseChess.Empty) == 1;
            }
        }

        private bool IsEat(ChineseChess[][] board, Step step)
        {
            var fromChess = board[step.FromX][step.FromY];
            var toChess = board[step.ToX][step.ToY];
            return toChess != ChineseChess.Empty && !fromChess.GetChessGroup().Contains(toChess);
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