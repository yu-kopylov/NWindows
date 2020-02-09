using System;
using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32Graphics : INativeGraphics
    {
        private readonly Gdi32ObjectCache objectCache;

        public Win32Graphics(Gdi32ObjectCache objectCache)
        {
            this.objectCache = objectCache;
        }

        public Size MeasureString(FontConfig font, string text)
        {
            IntPtr hdc = Win32API.GetDCChecked(IntPtr.Zero);
            try
            {
                IntPtr fontPtr = objectCache.GetFont(font);
                IntPtr oldFont = Gdi32API.SelectObjectChecked(hdc, fontPtr);
                try
                {
                    Gdi32API.GetTextExtentPoint32W(hdc, text, text.Length, out var size);
                    return new Size(size.cx, size.cy);
                }
                finally
                {
                    if (oldFont != IntPtr.Zero)
                    {
                        Gdi32API.SelectObjectChecked(hdc, fontPtr);
                    }
                }
            }
            finally
            {
                Win32API.ReleaseDCChecked(IntPtr.Zero, hdc);
            }
        }
    }
}