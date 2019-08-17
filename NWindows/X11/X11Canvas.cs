using System;
using System.Drawing;
using System.Globalization;
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
            var xftFont = objectCache.GetXftFont(font);
            var fontInfo = Marshal.PtrToStructure<XftFont>(xftFont);

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
                        byte[] utf32Text = Encoding.UTF32.GetBytes(text);
                        LibXft.XftTextExtents32(display, xftFont, utf32Text, utf32Text.Length / 4, out var extents);
                        LibXft.XftDrawString32(xftDraw, xftColorPtr, xftFont, x, y + fontInfo.ascent, utf32Text, utf32Text.Length / 4);
                        if (font.IsUnderline)
                        {
                            int lineHeight = Convert.ToInt32(Math.Max(font.Size / 10, 1));
                            LibXRender.XRenderFillRectangle(
                                display, PictOp.PictOpOver, pictureId, ref xColor,
                                x,
                                y + fontInfo.ascent + (fontInfo.descent - lineHeight) / 2,
                                extents.width,
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
                                extents.width,
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
    }
}