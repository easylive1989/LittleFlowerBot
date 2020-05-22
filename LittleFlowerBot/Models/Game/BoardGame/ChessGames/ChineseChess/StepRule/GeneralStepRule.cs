using System;
using System.Linq;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public class GeneralStepRule : IStepRule
    {
        
        public bool IsMatch(ChineseChess[][] board, Step step)
        {
            if (IsKingToKing(board, step))
            {
                return true;
            }

            if (IsOutPalace(board, step))
            {
                return false;
            }

            return IsOneStep(step);
        }

        private bool IsOutPalace(ChineseChess[][] board, Step step)
        {
            return step.ToY <= 2 || step.ToY >= 6 ||
                   (board[step.FromX][step.FromY] == ChineseChess.General && step.FromX >= 3) ||
                   (board[step.FromX][step.FromY] == ChineseChess.King && step.FromX <= 6);
        }

        private static bool IsOneStep(Step step)
        {
            return Math.Abs(step.ToX - step.FromX) + Math.Abs(step.ToY - step.FromY) == 1;
        }

        private static bool IsKingToKing(ChineseChess[][] board, Step step)
        {
            return (IsFromGeneralToKing(board, step) || IsFromKingToGeneral(board, step)) &&
                   step.FromY == step.ToY &&
                   IsNoChessBetweenKings(board, step);
        }

        private static bool IsFromKingToGeneral(ChineseChess[][] board, Step step)
        {
            return board[step.FromX][step.FromY] == ChineseChess.King && board[step.ToX][step.ToY] == ChineseChess.General;
        }

        private static bool IsFromGeneralToKing(ChineseChess[][] board, Step step)
        {
            return board[step.FromX][step.FromY] ==  ChineseChess.General && board[step.ToX][step.ToY] == ChineseChess.King;
        }

        private static bool IsNoChessBetweenKings(ChineseChess[][] board, Step step)
        {
            return Enumerable.Range(Math.Min(step.FromX, step.ToX), Math.Abs(step.ToX - step.FromX))
                .Where(x => board[x][step.FromY] != ChineseChess.General && board[x][step.FromY] != ChineseChess.King)
                .All(x => board[x][step.FromY] == ChineseChess.Empty);
        }
    }
}