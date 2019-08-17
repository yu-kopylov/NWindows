using System.Drawing;

namespace NWindows
{
    internal static class ColorExtensions
    {
        public static bool IsFullyOpaque(this Color color)
        {
            return color.A == 0xFF;
        }
    }
}