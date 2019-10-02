using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Graphics : INativeGraphics
    {
        private readonly IntPtr display;
        private readonly int screen;

        public X11Graphics(IntPtr display, int screen)
        {
            this.display = display;
            this.screen = screen;
        }

        public Size MeasureString(FontConfig font, string text)
        {
            using (X11ObjectCache objectCache = new X11ObjectCache(display, screen))
            {
                XftFontExt fontExt = objectCache.GetXftFont(font);
                byte[] utf32Text = Encoding.UTF32.GetBytes(text);

                int width = 0, lastWidth = 0, lastOffset = 0;

                GCHandle utf32TextHandle = GCHandle.Alloc(utf32Text, GCHandleType.Pinned);
                try
                {
                    IntPtr utf32TextPtr = utf32TextHandle.AddrOfPinnedObject();
                    foreach (var range in fontExt.GetRanges(utf32Text))
                    {
                        LibXft.XftTextExtents32(
                            display,
                            range.Font,
                            utf32TextPtr + range.Start,
                            (range.End - range.Start) / 4,
                            out var extents
                        );
                        width += extents.xOff;
                    }
                }
                finally
                {
                    utf32TextHandle.Free();
                }

                var fontInfo = Marshal.PtrToStructure<XftFont>(fontExt.MainFont);
                return new Size(width, fontInfo.height);
            }
        }
    }
}