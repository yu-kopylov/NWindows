using System.Drawing;

namespace NWindows
{
    internal interface INativeWindow
    {
        void SetTitle(string title);
        void Invalidate(Rectangle area);
    }
}