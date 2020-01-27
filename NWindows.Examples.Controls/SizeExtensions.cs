using System.Drawing;

namespace NWindows.Examples.Controls
{
    public static class SizeExtensions
    {
        public static bool HasZeroArea(this Size size)
        {
            return size.Width == 0 || size.Height == 0;
        }
    }
}