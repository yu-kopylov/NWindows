using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NWindows.X11
{
    public class X11ObjectCache : IDisposable
    {
        private readonly IntPtr display;
        private readonly int screen;

        private readonly Dictionary<FontConfig, IntPtr> fonts = new Dictionary<FontConfig, IntPtr>(X11FontConfigComparer.Instance);

        public X11ObjectCache(IntPtr display, int screen)
        {
            this.display = display;
            this.screen = screen;
        }

        public void Dispose()
        {
            foreach (IntPtr fontPtr in fonts.Values)
            {
                LibXft.XftFontClose(display, fontPtr);
            }
        }

        public IntPtr GetXftFont(FontConfig font)
        {
            if (fonts.TryGetValue(font, out IntPtr fontPtr))
            {
                return fontPtr;
            }

            fontPtr = LibXft.XftFontOpenName(display, screen, GetXftFontConfig(font));
            fonts.Add(font, fontPtr);
            return fontPtr;
        }

        private static byte[] GetXftFontConfig(FontConfig font)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(font.FontFamily);

            if (font.IsBold)
            {
                sb.Append(":bold");
            }

            if (font.IsItalic)
            {
                sb.Append(":italic");
            }

            sb.Append(":pixelsize=");
            sb.Append(font.Size.ToString("0.0", NumberFormatInfo.InvariantInfo));

            sb.Append('\0');

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}