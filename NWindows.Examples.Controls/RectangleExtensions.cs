using System.Drawing;

namespace NWindows.Examples.Controls
{
    public static class RectangleExtensions
    {
        public static bool HasZeroArea(this Rectangle rect)
        {
            return rect.Width == 0 || rect.Height == 0;
        }
    }
}