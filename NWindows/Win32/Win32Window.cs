using System;
using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    public class Win32Window : INativeWindow
    {
        private readonly IntPtr windowHandle;
        private readonly INativeWindowStartupInfo startupInfo;

        internal Win32Window(IntPtr windowHandle, INativeWindowStartupInfo startupInfo)
        {
            this.windowHandle = windowHandle;
            this.startupInfo = startupInfo;
        }

        internal IntPtr WindowHandle
        {
            get { return windowHandle; }
        }

        internal INativeWindowStartupInfo StartupInfo
        {
            get { return startupInfo; }
        }

        public void SetTitle(string title)
        {
            Win32API.SetWindowTextW(windowHandle, title);
        }

        public void Invalidate(Rectangle area)
        {
            RECT rect = new RECT {left = area.Left, top = area.Top, right = area.Right, bottom = area.Bottom};
            Win32API.InvalidateRect(windowHandle, ref rect, 0);
        }
    }
}