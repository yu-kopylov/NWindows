﻿using System.Drawing;

namespace NWindows.Examples
{
    public class OffsetCanvas : ICanvas
    {
        private readonly ICanvas canvas;
        private readonly int xOffset;
        private readonly int yOffset;

        public OffsetCanvas(ICanvas canvas, int xOffset, int yOffset)
        {
            this.canvas = canvas;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            canvas.FillRectangle(color, x + xOffset, y + yOffset, width, height);
        }

        public void DrawString(Color color, FontConfig font, int x, int y, string text)
        {
            canvas.DrawString(color, font, x + xOffset, y + yOffset, text);
        }
    }
}