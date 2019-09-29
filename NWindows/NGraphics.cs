using System.Drawing;
using NWindows.NativeApi;

namespace NWindows
{
    public class NGraphics
    {
        private readonly INativeGraphics nativeGraphics;

        public NGraphics(INativeGraphics nativeGraphics)
        {
            this.nativeGraphics = nativeGraphics;
        }

        public Size MeasureText(FontConfig fontConfig, string text)
        {
            return nativeGraphics.MeasureText(fontConfig, text);
        }
    }
}