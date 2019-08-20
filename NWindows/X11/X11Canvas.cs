using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace NWindows.X11
{
    internal class X11Canvas : ICanvas, IDisposable
    {
        private readonly IntPtr display;
        private readonly int screenNum;
        private readonly X11ObjectCache objectCache;
        private readonly IntPtr visual;
        private readonly ulong colormap;
        private readonly ulong windowId;
        private readonly ulong pictureId;

        private X11Canvas(
            IntPtr display,
            int screenNum,
            X11ObjectCache objectCache,
            IntPtr visual,
            ulong colormap,
            ulong windowId,
            ulong pictureId
        )
        {
            this.display = display;
            this.screenNum = screenNum;
            this.objectCache = objectCache;
            this.visual = visual;
            this.colormap = colormap;
            this.windowId = windowId;
            this.pictureId = pictureId;
        }

        public static X11Canvas CreateForWindow(
            IntPtr display,
            int screenNum,
            X11ObjectCache objectCache,
            IntPtr visual,
            ulong colormap,
            IntPtr pictFormatPtr,
            ulong windowId
        )
        {
            XRenderPictureAttributes attr = new XRenderPictureAttributes();
            ulong pictureId = LibXRender.XRenderCreatePicture
            (
                display,
                windowId,
                pictFormatPtr,
                0,
                ref attr
            );
            return new X11Canvas(display, screenNum, objectCache, visual, colormap, windowId, pictureId);
        }

        public void Dispose()
        {
            LibXRender.XRenderFreePicture(display, pictureId);
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            if (width < 0 || height < 0)
            {
                return;
            }

            XRenderColor xColor = new XRenderColor(color);
            LibXRender.XRenderFillRectangle(display, PictOp.PictOpOver, pictureId, ref xColor, x, y, (uint) width, (uint) height);
        }

        public void DrawString(Color color, FontConfig font, int x, int y, string text)
        {
            var xftDraw = LibXft.XftDrawCreate(display, windowId, visual, colormap);
            try
            {
                var xftColorPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XftColor>());
                try
                {
                    XRenderColor xColor = new XRenderColor(color);
                    LibXft.XftColorAllocValue(
                        display,
                        visual,
                        colormap,
                        xColor,
                        xftColorPtr
                    );

                    try
                    {
                        XftFontExt fontExt = objectCache.GetXftFont(font);
                        var fontInfo = Marshal.PtrToStructure<XftFont>(fontExt.MainFont);

                        int xOffset = DrawString(xftDraw, xftColorPtr, fontExt, x, y + fontInfo.ascent, text);

                        if (font.IsUnderline)
                        {
                            int lineHeight = Convert.ToInt32(Math.Max(font.Size / 10, 1));
                            LibXRender.XRenderFillRectangle(
                                display, PictOp.PictOpOver, pictureId, ref xColor,
                                x,
                                y + fontInfo.ascent + (fontInfo.descent - lineHeight) / 2,
                                (uint) xOffset,
                                (uint) lineHeight
                            );
                        }

                        if (font.IsStrikeout)
                        {
                            int lineHeight = Convert.ToInt32(Math.Max(font.Size / 20, 1));
                            LibXRender.XRenderFillRectangle
                            (
                                display, PictOp.PictOpOver, pictureId, ref xColor,
                                x,
                                y + fontInfo.ascent - (2 * fontInfo.ascent + 3 * lineHeight) / 6,
                                (uint) xOffset,
                                (uint) lineHeight
                            );
                        }
                    }
                    finally
                    {
                        LibXft.XftColorFree(display, visual, colormap, xftColorPtr);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(xftColorPtr);
                }
            }
            finally
            {
                LibXft.XftDrawDestroy(xftDraw);
            }
        }

        public void DrawImage(IImage image, int x, int y)
        {
            // todo: implement
        }

        private int DrawString(IntPtr xftDraw, IntPtr xftColorPtr, XftFontExt fontExt, int x, int y, string text)
        {
            byte[] utf32Text = Encoding.UTF32.GetBytes(text);
            IntPtr utf32TextPtr = Marshal.AllocHGlobal(utf32Text.Length);
            try
            {
                Marshal.Copy(utf32Text, 0, utf32TextPtr, utf32Text.Length);

                IntPtr rangeFont = fontExt.MainFont;
                int rangeStart = 0;
                int xOffset = 0;

                for (int i = 0; i < utf32Text.Length; i += 4)
                {
                    int codePoint = utf32Text[i + 3] << 24 | utf32Text[i + 2] << 16 | utf32Text[i + 1] << 8 | utf32Text[i];
                    IntPtr charFont = fontExt.GetFontByCodePoint(codePoint);

                    if (charFont != rangeFont)
                    {
                        xOffset += DrawStringRange(xftDraw, xftColorPtr, rangeFont, x + xOffset, y, utf32TextPtr, rangeStart, i);

                        rangeFont = charFont;
                        rangeStart = i;
                    }
                }

                xOffset += DrawStringRange(xftDraw, xftColorPtr, rangeFont, x + xOffset, y, utf32TextPtr, rangeStart, utf32Text.Length);
                return xOffset;
            }
            finally
            {
                Marshal.FreeHGlobal(utf32TextPtr);
            }
        }

        private int DrawStringRange(
            IntPtr xftDraw,
            IntPtr xftColorPtr,
            IntPtr font,
            int x,
            int y,
            IntPtr utf32TextPtr,
            int rangeStart,
            int rangeEnd
        )
        {
            if (rangeStart >= rangeEnd)
            {
                return 0;
            }

            LibXft.XftTextExtents32(
                display,
                font,
                utf32TextPtr + rangeStart,
                (rangeEnd - rangeStart) / 4,
                out var extents
            );
            LibXft.XftDrawString32(
                xftDraw,
                xftColorPtr,
                font,
                x,
                y,
                utf32TextPtr + rangeStart,
                (rangeEnd - rangeStart) / 4
            );
            return extents.xOff;
        }
    }
}