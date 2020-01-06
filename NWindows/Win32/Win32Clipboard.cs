using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32Clipboard : INativeClipboard
    {
        static readonly int[] delays = {0, 10, 20, 20, 50, 100, 100, 100, 100};

        public void PutText(string text)
        {
            if (!Open())
            {
                // Technically this is a reason to throw exception, but it should be rare and it will indicate
                // that clipboard is being heavily used and content of the clipboard might be unpredictable even if Open succeeds.
                return;
            }

            IntPtr dataHandle = IntPtr.Zero;

            try
            {
                Win32API.EmptyClipboard();

                dataHandle = Win32API.GlobalAlloc(GlobalAllocFlags.GMEM_MOVEABLE, (IntPtr) ((text.Length + 1) * sizeof(char)));
                if (dataHandle == IntPtr.Zero)
                {
                    throw new IOException("Failed to allocate memory for the clipboard data.");
                }

                IntPtr dataPtr = Win32API.GlobalLock(dataHandle);
                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, dataPtr, text.Length);
                    Marshal.WriteInt16(dataPtr, sizeof(char) * text.Length, '\0');
                }
                finally
                {
                    Win32API.GlobalUnlock(dataHandle);
                }

                if (Win32API.SetClipboardData(Win32ClipboardFormat.CF_UNICODETEXT, dataHandle) != IntPtr.Zero)
                {
                    // If SetClipboardData succeeds, then handle ownership is transferred to the system and it should not be freed by the application.
                    dataHandle = IntPtr.Zero;
                }
            }
            finally
            {
                Win32API.CloseClipboard();

                if (dataHandle != IntPtr.Zero)
                {
                    Win32API.GlobalFree(dataHandle);
                }
            }
        }

        public bool TryGetText(out string text)
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

                IntPtr dataHandle = Win32API.GetClipboardData((Win32ClipboardFormat) format);
                if (dataHandle == IntPtr.Zero)
                {
                    text = null;
                    return false;
                }

                IntPtr dataPtr = Win32API.GlobalLock(dataHandle);
                if (dataPtr == IntPtr.Zero)
                {
                    text = null;
                    return false;
                }

                try
                {
                    text = Marshal.PtrToStringUni(dataPtr);
                    return true;
                }
                finally
                {
                    Win32API.GlobalUnlock(dataHandle);
                }
            }
            finally
            {
                Win32API.CloseClipboard();
            }
        }

        private bool Open()
        {
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