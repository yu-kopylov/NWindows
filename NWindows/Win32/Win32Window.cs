using System;
using System.Drawing;

namespace NWindows.Win32
{
    public class Win32Window : INativeWindow
    {
        private readonly IntPtr windowHandle;

        public Win32Window(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;
        }

        public void SetTitle(string title)
        {
            Win32API.SetWindowTextW(windowHandle, title);
        }

        public void Invalidate(Rectangle area)
        {
            // todo: check +/-1 in width/height calculation
            RECT rect = new RECT {left = area.Left, top = area.Top, right = area.Right, bottom = area.Bottom};
            Win32API.InvalidateRect(windowHandle, ref rect, 0);
        }
    }
}