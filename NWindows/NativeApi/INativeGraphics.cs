using System.Drawing;

namespace NWindows.NativeApi
{
    public interface INativeGraphics
    {
        Size MeasureString(FontConfig font, string text);
    }
}