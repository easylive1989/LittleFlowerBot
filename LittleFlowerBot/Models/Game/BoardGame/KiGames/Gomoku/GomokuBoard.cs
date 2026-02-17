using System;
using System.Text;
using SkiaSharp;

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

        public override byte[] GetBoardImage()
        {
            const int cellSize = 40;
            const int marginLeft = 55;
            const int marginTop = 50;
            const int stoneRadius = 17;

            int boardWidth = (Column - 1) * cellSize;
            int boardHeight = (Row - 1) * cellSize;
            int width = marginLeft + boardWidth + 30;
            int height = marginTop + boardHeight + 30;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            // Background (wood color)
            canvas.Clear(new SKColor(0xDC, 0xB3, 0x5C));

            using var gridPaint = new SKPaint
            {
                Color = new SKColor(0x30, 0x20, 0x00),
                StrokeWidth = 1.5f,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            // Draw grid lines
            for (int i = 0; i < Row; i++)
            {
                float y = marginTop + i * cellSize;
                canvas.DrawLine(marginLeft, y, marginLeft + boardWidth, y, gridPaint);
            }
            for (int j = 0; j < Column; j++)
            {
                float x = marginLeft + j * cellSize;
                canvas.DrawLine(x, marginTop, x, marginTop + boardHeight, gridPaint);
            }

            // Draw star points (天元 and corners)
            using var starPaint = new SKPaint { Color = new SKColor(0x30, 0x20, 0x00), Style = SKPaintStyle.Fill, IsAntialias = true };
            int[][] starPoints = { new[] { 3, 3 }, new[] { 3, 11 }, new[] { 11, 3 }, new[] { 11, 11 }, new[] { 7, 7 } };
            foreach (var sp in starPoints)
            {
                float sx = marginLeft + sp[1] * cellSize;
                float sy = marginTop + sp[0] * cellSize;
                canvas.DrawCircle(sx, sy, 4, starPaint);
            }

            // Draw column labels (a-o)
            var typeface = SKTypeface.FromFamilyName("Noto Sans CJK TC")
                           ?? SKTypeface.FromFamilyName("WenQuanYi Micro Hei")
                           ?? SKTypeface.Default;
            using var labelFont = new SKFont(typeface, 18);
            using var labelPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
            for (int j = 0; j < Column; j++)
            {
                float x = marginLeft + j * cellSize;
                string label = ((char)('a' + j)).ToString();
                float tw = labelFont.MeasureText(label, labelPaint);
                canvas.DrawText(label, x - tw / 2, marginTop - 16, labelFont, labelPaint);
            }

            // Draw row labels (1-15)
            for (int i = 0; i < Row; i++)
            {
                float y = marginTop + i * cellSize + 6;
                string label = $"{i + 1}";
                float tw = labelFont.MeasureText(label, labelPaint);
                canvas.DrawText(label, marginLeft - tw - 10, y, labelFont, labelPaint);
            }

            // Draw stones
            using var blackFill = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill, IsAntialias = true };
            using var whiteFill = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill, IsAntialias = true };
            using var stoneStroke = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 1.5f, IsAntialias = true };
            using var lastMoveMark = new SKPaint { Color = new SKColor(0xCC, 0x00, 0x00), Style = SKPaintStyle.Fill, IsAntialias = true };

            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    var ki = GameBoardArray[i][j];
                    if (ki == Ki.Empty) continue;

                    float cx = marginLeft + j * cellSize;
                    float cy = marginTop + i * cellSize;

                    if (ki == Ki.Circle)
                    {
                        canvas.DrawCircle(cx, cy, stoneRadius, blackFill);
                    }
                    else
                    {
                        canvas.DrawCircle(cx, cy, stoneRadius, whiteFill);
                        canvas.DrawCircle(cx, cy, stoneRadius, stoneStroke);
                    }

                    // Last move indicator
                    if (i == CurMoveX && j == CurMoveY)
                    {
                        canvas.DrawCircle(cx, cy, 5, lastMoveMark);
                    }
                }
            }

            // Encode to PNG
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}