using System.Drawing;

namespace NWindows.NativeApi
{
    internal interface INativeWindow
    {
        void SetTitle(string title);
        void Invalidate(Rectangle area);
    }
}