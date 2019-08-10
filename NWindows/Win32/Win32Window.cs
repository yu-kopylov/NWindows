using System;

namespace NWindows.Win32
{
    public class Win32Window : INativeWindow
    {
        private readonly IntPtr windowHandle;

        public Win32Window(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;
        }
    }
}