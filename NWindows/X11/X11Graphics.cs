using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Graphics : INativeGraphics
    {
        public Size MeasureString(FontConfig font, string text)
        {
            // todo: implement
            return new Size(text.Length * 8, 16);
        }
    }
}