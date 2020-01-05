using System;
using System.Runtime.InteropServices;
using System.Threading;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32Clipboard : INativeClipboard
    {
        static readonly int[] delays = {0, 10, 20, 20, 50, 100, 100, 100, 100};

        public bool GetText(out string text)
        {
            if (!Open())
            {
                text = null;
                return false;
            }

            try
            {
                var formats = new[] {Win32ClipboardFormat.CF_UNICODETEXT};
                int format = Win32API.GetPriorityClipboardFormat(formats, formats.Length);
                if (format <= 0)
                {
                    text = null;
                    return false;
                }

                IntPtr clipboardDataPtr = Win32API.GetClipboardData((Win32ClipboardFormat) format);
                if (clipboardDataPtr == IntPtr.Zero)
                {
                    text = null;
                    return false;
                }

                IntPtr strPtr = Win32API.GlobalLock(clipboardDataPtr);
                if (strPtr == IntPtr.Zero)
                {
                    text = null;
                    return false;
                }

                try
                {
                    text = Marshal.PtrToStringUni(strPtr);
                    return true;
                }
                finally
                {
                    Win32API.GlobalUnlock(clipboardDataPtr);
                }
            }
            finally
            {
                Win32API.CloseClipboard();
            }
        }

        private bool Open()
        {
            // todo: are delays useful?
            foreach (int delay in delays)
            {
                if (delay != 0)
                {
                    Thread.Sleep(delay);
                }

                if (Win32API.OpenClipboard(IntPtr.Zero) != 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}