using System;
using System.Collections.Generic;

namespace NWindows.X11
{
    internal class X11ObjectCache : IDisposable
    {
        private readonly IntPtr display;
        private readonly int screen;

        private readonly Dictionary<FontConfig, XftFontExt> fonts = new Dictionary<FontConfig, XftFontExt>(X11FontConfigComparer.Instance);

        public X11ObjectCache(IntPtr display, int screen)
        {
            this.display = display;
            this.screen = screen;
        }

        public void Dispose()
        {
            foreach (XftFontExt font in fonts.Values)
            {
                font.Dispose();
            }
        }

        public XftFontExt GetXftFont(FontConfig fontConfig)
        {
            if (fonts.TryGetValue(fontConfig, out XftFontExt font))
            {
                return font;
            }

            font = XftFontExt.Create(fontConfig, display, screen);
            fonts.Add(fontConfig, font);
            return font;
        }
    }
}