using System.Drawing;

namespace NWindows.NativeApi
{
    public interface INativeGraphics
    {
        Size MeasureText(FontConfig font, string text);
    }
}