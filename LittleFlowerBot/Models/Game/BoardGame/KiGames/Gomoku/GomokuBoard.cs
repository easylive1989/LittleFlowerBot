using System;
using System.Text;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku
{
    [Serializable]
    public class GomokuBoard : KiBoard
    {
        private static int ROW_COUNT = 15;
        private static int COLUMN_COUNT = 15;

        public GomokuBoard() : base(ROW_COUNT, COLUMN_COUNT)
        {
        }

        public override bool IsDraw()
        {
            return GetTurn() >= ROW_COUNT * COLUMN_COUNT;
        }

        protected override bool IsSomeoneWin()
        {
            var curMoveX = CurMoveX;
            var curMoveY = CurMoveY;
            var curChess = this[curMoveX, curMoveY];

            if (curChess == Ki.Empty)
            {
                return false;
            }

            int verticalCount = VerifyVertical(curChess, curMoveX, curMoveY);
            if (verticalCount >= 5)
            {
                return true;
            }

            int horizontalCount = VerifyHorizontal(curChess, curMoveX, curMoveY);
            if (horizontalCount >= 5)
            {
                return true;
            }

            int slopeCount = VerifySlope(curChess, curMoveX, curMoveY);
            if (slopeCount >= 5)
            {
                return true;
            }

            int antiSlopeCount = VerifyAntiSlope(curChess, curMoveX, curMoveY);
            if (antiSlopeCount >= 5)
            {
                return true;
            }

            return false;;
        }
        
        private int VerifyAntiSlope(Ki chess, int x, int y, bool? toNegative = null)
        {
            if (x < 0 || y < 0 || x >= ROW_COUNT || y >= COLUMN_COUNT)
            {
                return 0;
            }

            if (this[x, y] == chess)
            {
                if (toNegative == null)
                    return VerifyAntiSlope(chess, x + 1, y - 1, true) + 1 + VerifyAntiSlope(chess, x - 1, y + 1, false);
                if (toNegative == true)
                    return VerifyAntiSlope(chess, x + 1, y - 1, true) + 1;
                return 1 + VerifyAntiSlope(chess, x - 1, y + 1, false);
            }
            return 0;
        }

        private int VerifySlope(Ki chess, int x, int y, bool? toNegative = null)
        {
            if (x < 0 || y < 0 || x >= ROW_COUNT || y >= COLUMN_COUNT)
            {
                return 0;
            }

            if (this[x, y] == chess)
            {
                if (toNegative == null)
                    return VerifySlope(chess, x - 1, y - 1, true) + 1 + VerifySlope(chess, x + 1, y + 1, false);
                if (toNegative == true)
                    return VerifySlope(chess, x - 1, y - 1, true) + 1;
                return 1 + VerifySlope(chess, x + 1, y + 1, false);
            }
            return 0;
        }

        private int VerifyHorizontal(Ki chess, int x, int y, bool? toNegative = null)
        {
            if (x < 0 || y < 0 || x >= ROW_COUNT || y >= COLUMN_COUNT)
            {
                return 0;
            }

            if (this[x, y] == chess)
            {
                if (toNegative == null)
                    return VerifyHorizontal(chess, x - 1, y, true) + 1 + VerifyHorizontal(chess, x + 1, y, false);
                else if (toNegative == true)
                    return VerifyHorizontal(chess, x - 1, y, true) + 1;
                else
                    return 1 + VerifyHorizontal(chess, x + 1, y, false);
            }
            return 0;
        }

        private int VerifyVertical(Ki chess, int x, int y, bool? toNegative = null)
        {
            if (x < 0 || y < 0 || x >= ROW_COUNT || y >= COLUMN_COUNT)
            {
                return 0;
            }

            if (this[x, y] == chess)
            {
                if (toNegative == null)
                    return VerifyVertical(chess, x, y - 1, true) + 1 + VerifyVertical(chess, x, y + 1, false);
                else if (toNegative == true)
                    return VerifyVertical(chess, x, y - 1, true) + 1;
                else
                    return 1 + VerifyVertical(chess, x, y + 1, false);
            }
            return 0;
        }

        public override string GetBoardString()
        {
            var builder = new StringBuilder();
            builder.Append("00ⒶⒷⒸⒹⒺⒻⒼⒽⒾⒿⓀⓁⓜⓃ⓪\n");
            for (int i = 0; i < Row; i++)
            {
                builder.Append($"{i + 1:D2}");
                for (int j = 0; j < Column; j++)
                {
                    switch (GameBoardArray[i][j])
                    {
                        case Ki.Circle:
                            if (i == CurMoveX && j == CurMoveY) builder.Append("★");
                            else builder.Append("●");
                            
                            break;

                        case Ki.Cross:
                            if (i == CurMoveX && j == CurMoveY) builder.Append("☆");
                            else builder.Append("○");
                            break;

                        case Ki.Empty:
                            builder.Append("┼");
                            break;
                    }
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}