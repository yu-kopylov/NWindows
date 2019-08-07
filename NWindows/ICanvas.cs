using System;
using System.Drawing;

namespace NWindows
{
    public interface ICanvas : IDisposable
    {
        void FillRectangle(Color color, int x, int y, int width, int height);
    }
}