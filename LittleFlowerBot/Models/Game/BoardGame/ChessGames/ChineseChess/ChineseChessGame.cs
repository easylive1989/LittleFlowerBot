using System.Text.RegularExpressions;
using LittleFlowerBot.Models.GameExceptions;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess
{
    public class ChineseChessGame : BoardGame<ChineseChess>
    {
        public ChineseChessGame()
        {
            GameBoard = new ChineseChessBoard();
        }

        protected override void Move(string cmd, Player player)
        {
            try
            {
                base.Move(cmd, player);
            }
            catch (NotYourChessException)
            {
                Render("不是你的棋子");
            }
            catch (MoveInvalidException)
            {
                Render("走法有誤");
            }
        }

        protected override bool IsCmdValid(string cmd)
        {
            return new Regex(@"^([1-9]|10),[a-i]>([1-9]|10),[a-i]$").IsMatch(cmd);
        }
    }
}