using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class W32Graphics : INativeGraphics
    {
        public Size MeasureText(FontConfig font, string text)
        {
            // todo: implement
            return new Size(text.Length * 8, 16);
        }
    }
}