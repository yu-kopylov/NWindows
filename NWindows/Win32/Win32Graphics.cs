using System;
using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32Graphics : INativeGraphics
    {
        public Size MeasureString(FontConfig font, string text)
        {
            IntPtr hdc = Win32API.GetDCChecked(IntPtr.Zero);
            try
            {
                using (Gdi32ObjectCache objectCache = new Gdi32ObjectCache())
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
            }
            finally
            {
                Win32API.ReleaseDCChecked(IntPtr.Zero, hdc);
            }
        }
    }
}