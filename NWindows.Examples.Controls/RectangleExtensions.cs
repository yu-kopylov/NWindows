using System.Collections.Generic;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public static class RectangleExtensions
    {
        public static bool HasZeroArea(this Rectangle rect)
        {
            return rect.Width == 0 || rect.Height == 0;
        }

        public static IEnumerable<Rectangle> Exclude(this Rectangle rect, Rectangle excludedArea)
        {
            var commonArea = Rectangle.Intersect(rect, excludedArea);

            if (commonArea.HasZeroArea())
            {
                yield return rect;
                yield break;
            }

            if (rect.Top < commonArea.Top)
            {
                yield return new Rectangle(rect.Left, rect.Top, rect.Width, commonArea.Top - rect.Top);
            }

            if (rect.Bottom > commonArea.Bottom)
            {
                yield return new Rectangle(rect.Left, commonArea.Bottom, rect.Width, rect.Bottom - commonArea.Bottom);
            }

            if (rect.Left < commonArea.Left)
            {
                yield return new Rectangle(rect.Left, commonArea.Top, commonArea.Left - rect.Left, commonArea.Height);
            }

            if (rect.Right > commonArea.Right)
            {
                yield return new Rectangle(commonArea.Right, commonArea.Top, rect.Right - commonArea.Right, commonArea.Height);
            }
        }
    }
}