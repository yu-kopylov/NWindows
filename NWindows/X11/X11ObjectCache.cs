using System;
using NWindows.Utils;

namespace NWindows.X11
{
    internal class X11ObjectCache
    {
        private readonly IntPtr display;
        private readonly int screen;

        private readonly Cache<FontConfig, XftFontExt> fonts;

        public X11ObjectCache(IntPtr display, int screen)
        {
            this.display = display;
            this.screen = screen;

            fonts = new Cache<FontConfig, XftFontExt>(64, CreateXftFont, f => f.Dispose(), X11FontConfigComparer.Instance);
        }

        public void Clear()
        {
            fonts.Clear();
        }

        public XftFontExt GetXftFont(FontConfig fontConfig)
        {
            return fonts.Get(fontConfig);
        }

        private XftFontExt CreateXftFont(FontConfig fontConfig)
        {
            return XftFontExt.Create(fontConfig, display, screen);
        }
    }
}