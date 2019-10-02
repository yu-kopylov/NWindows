using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Canvas : INativeCanvas, IDisposable
    {
        private readonly IntPtr display;
        private readonly int screenNum;
        private readonly X11ObjectCache objectCache;
        private readonly IntPtr visual;
        private readonly ulong colormap;
        private readonly IntPtr pictFormatPtr;
        private readonly ulong windowId;
        private readonly ulong pictureId;
        private readonly IntPtr xftDraw;

        private X11Canvas(
            IntPtr display,
            int screenNum,
            X11ObjectCache objectCache,
            IntPtr visual,
            ulong colormap,
            IntPtr pictFormatPtr,
            ulong windowId,
            ulong pictureId,
            IntPtr xftDraw
        )
        {
            this.display = display;
            this.screenNum = screenNum;
            this.objectCache = objectCache;
            this.visual = visual;
            this.colormap = colormap;
            this.pictFormatPtr = pictFormatPtr;
            this.windowId = windowId;
            this.pictureId = pictureId;
            this.xftDraw = xftDraw;
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

            IntPtr xftDraw = IntPtr.Zero;
            X11Canvas canvas = null;

            try
            {
                xftDraw = LibXft.XftDrawCreate(display, windowId, visual, colormap);
                canvas = new X11Canvas(display, screenNum, objectCache, visual, colormap, pictFormatPtr, windowId, pictureId, xftDraw);
                xftDraw = IntPtr.Zero;
            }
            finally
            {
                if (canvas == null)
                {
                    if (xftDraw != IntPtr.Zero)
                    {
                        LibXft.XftDrawDestroy(xftDraw);
                    }

                    LibXRender.XRenderFreePicture(display, pictureId);
                }
            }

            return canvas;
        }

        public void Dispose()
        {
            LibXft.XftDrawDestroy(xftDraw);
            LibXRender.XRenderFreePicture(display, pictureId);
        }

        public void SetClipRectangle(int x, int y, int width, int height)
        {
            XRectangle[] rectangles = {new XRectangle(x, y, width, height)};
            LibXRender.XRenderSetPictureClipRectangles(display, pictureId, 0, 0, rectangles, 1);
            LibXft.XftDrawSetClipRectangles(xftDraw, 0, 0, rectangles, 1);
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

        private int DrawString(IntPtr xftDraw, IntPtr xftColorPtr, XftFontExt fontExt, int x, int y, string text)
        {
            byte[] utf32Text = Encoding.UTF32.GetBytes(text);
            GCHandle utf32TextHandle = GCHandle.Alloc(utf32Text, GCHandleType.Pinned);
            try
            {
                IntPtr utf32TextPtr = utf32TextHandle.AddrOfPinnedObject();

                int xOffset = 0;
                foreach (var range in fontExt.GetRanges(utf32Text))
                {
                    xOffset += DrawStringRange(xftDraw, xftColorPtr, range.Font, x + xOffset, y, utf32TextPtr, range.Start, range.End);
                }

                return xOffset;
            }
            finally
            {
                utf32TextHandle.Free();
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

        public void DrawImage(INativeImage image, int x, int y)
        {
            // todo: allow null?
            X11Image x11Image = (X11Image) image;

            var pixmapId = LibX11.XCreatePixmap(
                display,
                windowId,
                (uint) x11Image.Width,
                (uint) x11Image.Height,
                X11Application.RequiredColorDepth
            );
            try
            {
                var gcValues = new XGCValues();
                var gc = LibX11.XCreateGC(display, pixmapId, 0, ref gcValues);
                try
                {
                    LibX11.XPutImage(display, pixmapId, gc, x11Image.XImage, 0, 0, 0, 0, (uint) x11Image.Width, (uint) x11Image.Height);
                }
                finally
                {
                    LibX11.XFreeGC(display, gc);
                }

                XRenderPictureAttributes attr = new XRenderPictureAttributes();
                var tempPictureId = LibXRender.XRenderCreatePicture(display, pixmapId, pictFormatPtr, 0, ref attr);
                try
                {
                    LibXRender.XRenderComposite
                    (
                        display,
                        PictOp.PictOpOver,
                        tempPictureId,
                        0,
                        pictureId,
                        0, 0,
                        0, 0,
                        x, y,
                        (uint) x11Image.Width, (uint) x11Image.Height
                    );
                }
                finally
                {
                    LibXRender.XRenderFreePicture(display, tempPictureId);
                }
            }
            finally
            {
                LibX11.XFreePixmap(display, pixmapId);
            }
        }
    }
}