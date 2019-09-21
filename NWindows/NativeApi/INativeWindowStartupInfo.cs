using System.Drawing;

namespace NWindows.NativeApi
{
    internal interface INativeWindowStartupInfo
    {
        string Title { get; }
        int Width { get; }
        int Height { get; }

        void OnCreate(INativeWindow nativeWindow);
        void OnKeyDown(NKeyCode keyCode, bool autoRepeat);
        void OnKeyUp(NKeyCode keyCode);
        void OnMouseMove(Point point);
        void OnPaint(INativeCanvas canvas, Rectangle area);
        void OnResize(Size clientArea);
    }
}