using System.Collections.Generic;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;

namespace LittleFlowerBot.Extensions
{
    public static class ChineseChessExtensions
    {
        public static readonly HashSet<ChineseChess> BlackChessGroup = new HashSet<ChineseChess>()
        {
            ChineseChess.General,
            ChineseChess.Guard,
            ChineseChess.Elephant,
            ChineseChess.Rook,
            ChineseChess.Horse,
            ChineseChess.Cannon,
            ChineseChess.Soldier
        };

        public static readonly HashSet<ChineseChess> RedChessGroup = new HashSet<ChineseChess>()
        {
            ChineseChess.King,
            ChineseChess.Adviser,
            ChineseChess.Minister,
            ChineseChess.Chariot,
            ChineseChess.Knight,
            ChineseChess.RedCannon,
            ChineseChess.Pawn,
        };
        
        public static HashSet<ChineseChess> GetChessGroup(this ChineseChess chess)
        {
            return BlackChessGroup.Contains(chess) ? BlackChessGroup : RedChessGroup;
        }
    }
}