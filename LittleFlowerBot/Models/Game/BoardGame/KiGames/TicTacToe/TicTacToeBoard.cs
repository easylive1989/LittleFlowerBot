using System;
using System.Text;
using SkiaSharp;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe
{
    [Serializable]
    public class TicTacToeBoard : KiBoard
    {
        public TicTacToeBoard() : base(3, 3)
        {
        }

        public override bool IsDraw()
        {
            return GetTurn() >= 9;
        }

        protected override bool IsSomeoneWin()
        {
            var gameBoard = this;
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[i, 0] == gameBoard[i, 1] &&
                    gameBoard[i, 1] == gameBoard[i, 2] &&
                    gameBoard[i, 0] != Ki.Empty)
                {
                    return true;
                }

                if (gameBoard[0, i] == gameBoard[1, i] &&
                    gameBoard[1, i] == gameBoard[2, i] &&
                    gameBoard[0, i] != Ki.Empty)
                {
                    return true;
                }
            }

            if (gameBoard[0, 0] == gameBoard[1, 1] &&
                gameBoard[1, 1] == gameBoard[2, 2] &&
                gameBoard[0, 0] != Ki.Empty)
            {                    
                return true;
            }

            if (gameBoard[0, 2] == gameBoard[1, 1] &&
                gameBoard[1, 1] == gameBoard[2, 0] &&
                gameBoard[0, 2] != Ki.Empty)
            {                    
                return true;
            }

            return false;
        }
        
        public override string GetBoardString()
        {
            var builder = new StringBuilder();
            builder.Append("00ⒶⒷⒸ\n");
            for (int i = 0; i < Row; i++)
            {
                builder.Append($"{i + 1:D2}");
                for (int j = 0; j < Column; j++)
                {
                    switch (GameBoardArray[i][j])
                    {
                        case Ki.Circle:
                            builder.Append("●");
                            break;

                        case Ki.Cross:
                            builder.Append("○");
                            break;

                        case Ki.Empty:
                            builder.Append("＿");
                            break;
                    }
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }

        public override byte[] GetBoardImage()
        {
            const int cellSize = 120;
            const int marginLeft = 50;
            const int marginTop = 45;
            const int symbolPadding = 20;

            int boardWidth = Column * cellSize;
            int boardHeight = Row * cellSize;
            int width = marginLeft + boardWidth + 30;
            int height = marginTop + boardHeight + 30;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            // Background
            canvas.Clear(new SKColor(0xF5, 0xF5, 0xF0));

            using var gridPaint = new SKPaint
            {
                Color = new SKColor(0x33, 0x33, 0x33),
                StrokeWidth = 3f,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            // Draw grid lines (2 horizontal, 2 vertical to make 3x3)
            for (int i = 1; i < Row; i++)
            {
                float y = marginTop + i * cellSize;
                canvas.DrawLine(marginLeft, y, marginLeft + boardWidth, y, gridPaint);
            }
            for (int j = 1; j < Column; j++)
            {
                float x = marginLeft + j * cellSize;
                canvas.DrawLine(x, marginTop, x, marginTop + boardHeight, gridPaint);
            }

            // Draw border
            canvas.DrawRect(marginLeft, marginTop, boardWidth, boardHeight, gridPaint);

            // Draw labels
            var typeface = SKTypeface.FromFamilyName("Noto Sans CJK TC")
                           ?? SKTypeface.FromFamilyName("WenQuanYi Micro Hei")
                           ?? SKTypeface.Default;
            using var labelFont = new SKFont(typeface, 20);
            using var labelPaint = new SKPaint { Color = new SKColor(0x33, 0x33, 0x33), IsAntialias = true };
            string[] colLabels = { "A", "B", "C" };
            for (int j = 0; j < Column; j++)
            {
                float x = marginLeft + j * cellSize + cellSize / 2f;
                float tw = labelFont.MeasureText(colLabels[j], labelPaint);
                canvas.DrawText(colLabels[j], x - tw / 2, marginTop - 12, labelFont, labelPaint);
            }
            for (int i = 0; i < Row; i++)
            {
                float y = marginTop + i * cellSize + cellSize / 2f + 7;
                string label = $"{i + 1}";
                float tw = labelFont.MeasureText(label, labelPaint);
                canvas.DrawText(label, marginLeft - tw - 12, y, labelFont, labelPaint);
            }

            // Draw pieces
            using var circlePaint = new SKPaint { Color = new SKColor(0x22, 0x22, 0xCC), Style = SKPaintStyle.Stroke, StrokeWidth = 6f, IsAntialias = true };
            using var crossPaint = new SKPaint { Color = new SKColor(0xCC, 0x22, 0x22), Style = SKPaintStyle.Stroke, StrokeWidth = 6f, IsAntialias = true };

            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    float cx = marginLeft + j * cellSize + cellSize / 2f;
                    float cy = marginTop + i * cellSize + cellSize / 2f;
                    float r = cellSize / 2f - symbolPadding;

                    switch (GameBoardArray[i][j])
                    {
                        case Ki.Circle:
                            canvas.DrawCircle(cx, cy, r, circlePaint);
                            break;
                        case Ki.Cross:
                            canvas.DrawLine(cx - r, cy - r, cx + r, cy + r, crossPaint);
                            canvas.DrawLine(cx + r, cy - r, cx - r, cy + r, crossPaint);
                            break;
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