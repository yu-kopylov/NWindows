using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NWindows.X11
{
    internal class XftFontExt : IDisposable
    {
        private const int MaxFontsCount = 8;

        private readonly FontConfig fontConfig;
        private readonly IntPtr display;
        private readonly int screen;
        private readonly List<IntPtr> fonts = new List<IntPtr>();
        private readonly Dictionary<int, IntPtr> fontsByCodePoint = new Dictionary<int, IntPtr>();

        private XftFontExt(FontConfig fontConfig, IntPtr display, int screen, IntPtr mainFont)
        {
            this.fontConfig = fontConfig;
            this.display = display;
            this.screen = screen;
            MainFont = mainFont;
            fonts.Add(mainFont);
        }

        public static XftFontExt Create(FontConfig fontConfig, IntPtr display, int screen)
        {
            IntPtr fontPtr = LibXft.XftFontOpenName(display, screen, GetXftFontConfig(fontConfig, -1));
            return new XftFontExt(fontConfig, display, screen, fontPtr);
        }

        public void Dispose()
        {
            foreach (IntPtr font in fonts)
            {
                LibXft.XftFontClose(display, font);
            }
        }

        public IntPtr MainFont { get; }

        public IntPtr GetFontByCodePoint(int codePoint)
        {
            if (fontsByCodePoint.TryGetValue(codePoint, out IntPtr fontPtr))
            {
                return fontPtr;
            }

            fontPtr = GetFontByCodePointNonCached(codePoint);
            fontsByCodePoint.Add(codePoint, fontPtr);
            return fontPtr;
        }

        private IntPtr GetFontByCodePointNonCached(int codePoint)
        {
            foreach (IntPtr font in fonts)
            {
                if (LibXft.XftCharExists(display, font, (uint) codePoint) != 0)
                {
                    return font;
                }
            }

            if (fonts.Count >= MaxFontsCount)
            {
                return MainFont;
            }

            IntPtr fontPtr = LibXft.XftFontOpenName(display, screen, GetXftFontConfig(fontConfig, codePoint));
            if (fontPtr == IntPtr.Zero)
            {
                return MainFont;
            }

            if (LibXft.XftCharExists(display, fontPtr, (uint) codePoint) == 0)
            {
                LibXft.XftFontClose(display, fontPtr);
                return MainFont;
            }

            fonts.Add(fontPtr);
            return fontPtr;
        }

        private static byte[] GetXftFontConfig(FontConfig font, int codePoint)
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

            if (codePoint >= 0)
            {
                sb.Append(":charset=");
                sb.Append(codePoint.ToString("X", NumberFormatInfo.InvariantInfo));
            }

            sb.Append('\0');

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}