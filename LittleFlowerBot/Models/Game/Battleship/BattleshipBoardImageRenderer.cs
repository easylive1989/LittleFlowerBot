using SkiaSharp;

namespace LittleFlowerBot.Models.Game.Battleship
{
    public static class BattleshipBoardImageRenderer
    {
        private const int CellSize = 40;
        private const int MarginLeft = 40;
        private const int MarginTop = 40;
        private const int GridSize = 10;

        public static byte[] RenderOwnGrid(PlayerState state)
        {
            return RenderGrid(state.OwnGrid, "己方棋盤", isOwnGrid: true);
        }

        public static byte[] RenderAttackGrid(PlayerState state)
        {
            return RenderGrid(state.AttackGrid, "攻擊棋盤", isOwnGrid: false);
        }

        private static byte[] RenderGrid(CellState[][] grid, string title, bool isOwnGrid)
        {
            int boardWidth = GridSize * CellSize;
            int boardHeight = GridSize * CellSize;
            int width = MarginLeft + boardWidth + 20;
            int height = MarginTop + boardHeight + 20;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            // Background
            canvas.Clear(new SKColor(0x1A, 0x3C, 0x5E));

            var typeface = SKTypeface.FromFamilyName("Noto Sans CJK TC")
                           ?? SKTypeface.FromFamilyName("WenQuanYi Micro Hei")
                           ?? SKTypeface.Default;

            // Title
            using var titleFont = new SKFont(typeface, 18);
            using var titlePaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
            canvas.DrawText(title, MarginLeft, MarginTop - 15, titleFont, titlePaint);

            // Grid lines
            using var gridPaint = new SKPaint
            {
                Color = new SKColor(0x60, 0x80, 0xA0),
                StrokeWidth = 1f,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            for (int i = 0; i <= GridSize; i++)
            {
                float y = MarginTop + i * CellSize;
                canvas.DrawLine(MarginLeft, y, MarginLeft + boardWidth, y, gridPaint);
                float x = MarginLeft + i * CellSize;
                canvas.DrawLine(x, MarginTop, x, MarginTop + boardHeight, gridPaint);
            }

            // Column labels (a-j)
            using var labelFont = new SKFont(typeface, 14);
            using var labelPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
            for (int j = 0; j < GridSize; j++)
            {
                string label = ((char)('a' + j)).ToString();
                float x = MarginLeft + j * CellSize + CellSize / 2f;
                float tw = labelFont.MeasureText(label, labelPaint);
                canvas.DrawText(label, x - tw / 2, MarginTop - 3, labelFont, labelPaint);
            }

            // Row labels (1-10)
            for (int i = 0; i < GridSize; i++)
            {
                string label = (i + 1).ToString();
                float y = MarginTop + i * CellSize + CellSize / 2f + 5;
                float tw = labelFont.MeasureText(label, labelPaint);
                canvas.DrawText(label, MarginLeft - tw - 5, y, labelFont, labelPaint);
            }

            // Cells
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    float cx = MarginLeft + j * CellSize + CellSize / 2f;
                    float cy = MarginTop + i * CellSize + CellSize / 2f;
                    var cell = grid[i][j];

                    DrawCell(canvas, cx, cy, cell, isOwnGrid);
                }
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        private static void DrawCell(SKCanvas canvas, float cx, float cy, CellState cell, bool isOwnGrid)
        {
            float half = CellSize / 2f - 2;

            switch (cell)
            {
                case CellState.Ship when isOwnGrid:
                    using (var shipPaint = new SKPaint
                    {
                        Color = new SKColor(0x60, 0x60, 0x60),
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true
                    })
                    {
                        canvas.DrawRect(cx - half, cy - half, half * 2, half * 2, shipPaint);
                    }
                    break;

                case CellState.Hit:
                    using (var hitFill = new SKPaint
                    {
                        Color = new SKColor(0xCC, 0x33, 0x33),
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true
                    })
                    {
                        canvas.DrawCircle(cx, cy, half * 0.6f, hitFill);
                    }
                    // X mark
                    using (var xPaint = new SKPaint
                    {
                        Color = SKColors.White,
                        StrokeWidth = 2f,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke
                    })
                    {
                        float s = half * 0.4f;
                        canvas.DrawLine(cx - s, cy - s, cx + s, cy + s, xPaint);
                        canvas.DrawLine(cx + s, cy - s, cx - s, cy + s, xPaint);
                    }
                    break;

                case CellState.Miss:
                    using (var missPaint = new SKPaint
                    {
                        Color = SKColors.White,
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true
                    })
                    {
                        canvas.DrawCircle(cx, cy, 4f, missPaint);
                    }
                    break;
            }
        }
    }
}
