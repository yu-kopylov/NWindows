﻿using System.Drawing;

namespace NWindows.Examples
{
    public abstract class Control
    {
        public Rectangle Area { get; set; }

        public abstract void Paint(ICanvas canvas, Rectangle area);
    }
}