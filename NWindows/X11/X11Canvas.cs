using System;
using System.Drawing;
using System.Linq;
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
        private readonly ulong drawableId;
        private readonly ulong pictureId;
        private readonly IntPtr xftDraw;

        private Point origin;
        private XRectangle[] clipRectangles;

        private X11Canvas(
            IntPtr display,
            int screenNum,
            X11ObjectCache objectCache,
            IntPtr visual,
            ulong colormap,
            IntPtr pictFormatPtr,
            ulong drawableId,
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
            this.drawableId = drawableId;
            this.pictureId = pictureId;
            this.xftDraw = xftDraw;
        }

        public static X11Canvas CreateForDrawable(
            IntPtr display,
            int screenNum,
            X11ObjectCache objectCache,
            IntPtr visual,
            ulong colormap,
            IntPtr pictFormatPtr,
            ulong drawableId
        )
        {
            const XRenderPictureAttributeMask attrMask =
                XRenderPictureAttributeMask.CPPolyEdge |
                XRenderPictureAttributeMask.CPPolyMode;

            XRenderPictureAttributes attr = new XRenderPictureAttributes
            {
                poly_edge = XRenderPolyEdge.Smooth,
                poly_mode = XRenderPolyMode.Imprecise
            };

            ulong pictureId = LibXRender.XRenderCreatePicture
            (
                display,
                drawableId,
                pictFormatPtr,
                attrMask,
                ref attr
            );

            IntPtr xftDraw = IntPtr.Zero;
            X11Canvas canvas = null;

            try
            {
                xftDraw = LibXft.XftDrawCreate(display, drawableId, visual, colormap);
                canvas = new X11Canvas(display, screenNum, objectCache, visual, colormap, pictFormatPtr, drawableId, pictureId, xftDraw);
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

        public void SetOrigin(int x, int y)
        {
            origin = new Point(x, y);
        }

        public void SetClipRectangle(int x, int y, int width, int height)
        {
            x -= origin.X;
            y -= origin.Y;

            clipRectangles = new[] {new XRectangle(x, y, width, height)};
            LibXRender.XRenderSetPictureClipRectangles(display, pictureId, 0, 0, clipRectangles, 1);
            LibXft.XftDrawSetClipRectangles(xftDraw, 0, 0, clipRectangles, 1);
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            // todo: <= 0 instead?
            if (width < 0 || height < 0)
            {
                return;
            }

            x -= origin.X;
            y -= origin.Y;

            XRenderColor xColor = new XRenderColor(color);
            LibXRender.XRenderFillRectangle(display, PictOp.PictOpOver, pictureId, ref xColor, x, y, (uint) width, (uint) height);
        }

        public void DrawString(Color color, FontConfig font, int x, int y, string text)
        {
            x -= origin.X;
            y -= origin.Y;

            var xftColorPtr = Marshal.AllocHGlobal(Marshal.SizeOf<XftColor>());
            try
            {
                XRenderColor xColor = new XRenderColor(color);
                LibXft.XftColorAllocValue(
                    display,
                    visual,
                    colormap,
                    ref xColor,
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
            // todo: reuse buffer?
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
            x -= origin.X;
            y -= origin.Y;

            // todo: allow null?
            X11Image x11Image = (X11Image) image;

            XRenderPictureAttributes attr = new XRenderPictureAttributes();
            var tempPictureId = LibXRender.XRenderCreatePicture(display, x11Image.PixmapId, pictFormatPtr, 0, ref attr);
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

        public void DrawPath(Color color, int width, Point[] points)
        {
            DrawPathWithXRenderCompositeTriangles(color, width, points);
        }

        private void DrawPathWithXLib(Color color, int width, Point[] points)
        {
            if (origin.X != 0 || origin.Y != 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = new Point(points[i].X - origin.X, points[i].Y - origin.Y);
                }
            }

            if (color.IsFullyOpaque())
            {
                DrawPathWithXLibDirectly(color, width, points);
                return;
            }

            // todo: handle points.Length <= 1

            int minX = points.Min(p => p.X) - width;
            int minY = points.Min(p => p.Y) - width;
            int maxX = points.Max(p => p.X) + width;
            int maxY = points.Max(p => p.Y) + width;

            var relativePoints = new Point[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                relativePoints[i] = new Point(point.X - minX, point.Y - minY);
            }

            WithExtraCanvas
            (
                new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1),
                canvas => canvas.DrawPathWithXLibDirectly(color, width, relativePoints)
            );
        }

        private void DrawPathWithXLibDirectly(Color color, int width, Point[] points)
        {
            var gcValues = new XGCValues();
            gcValues.cap_style = X11CapStyle.CapRound;
            gcValues.join_style = X11JoinStyle.JoinRound;
            gcValues.foreground = ToPArgb(color);
            gcValues.line_width = width;

            var gcValueMask = XGCValueMask.GCCapStyle | XGCValueMask.GCForeground | XGCValueMask.GCJoinStyle | XGCValueMask.GCLineWidth;

            var gc = LibX11.XCreateGC(display, drawableId, gcValueMask, ref gcValues);
            try
            {
                if (clipRectangles != null)
                {
                    LibX11.XSetClipRectangles(display, gc, 0, 0, clipRectangles, clipRectangles.Length, 0);
                }

                XPoint[] xPoints = new XPoint[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    xPoints[i] = new XPoint(point.X, point.Y);
                }

                LibX11.XDrawLines(display, drawableId, gc, xPoints, xPoints.Length, X11CoordinateMode.CoordModeOrigin);
            }
            finally
            {
                LibX11.XFreeGC(display, gc);
            }
        }

        private void DrawPathWithXRenderCompositeTriangles(Color color, int width, Point[] points)
        {
            if (width == 0)
            {
                width = 1;
            }

            XRenderColor xColor = new XRenderColor(color);
            // todo: cache brush
            ulong brush = LibXRender.XRenderCreateSolidFill(display, ref xColor);
            try
            {
                XTriangle[] triangles = new XTriangle[(points.Length - 1) * 2];
                for (int i = 1, t = 0; i < points.Length; i++)
                {
                    Point p1 = points[i - 1];
                    Point p2 = points[i];

                    p1.Offset(-origin.X, -origin.Y);
                    p2.Offset(-origin.X, -origin.Y);

                    int dx = p2.X - p1.X;
                    int dy = p2.Y - p1.Y;
                    double len = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    // todo: handle 0 points and zero-length segments
                    int ox = Convert.ToInt32(-dy / len * 32768 * width);
                    int oy = Convert.ToInt32(dx / len * 32768 * width);

                    triangles[t++] = new XTriangle
                    {
                        p1 = new XPointFixed {x = (p1.X << 16) + ox, y = (p1.Y << 16) + oy},
                        p2 = new XPointFixed {x = (p2.X << 16) + ox, y = (p2.Y << 16) + oy},
                        p3 = new XPointFixed {x = (p1.X << 16) - ox, y = (p1.Y << 16) - oy}
                    };
                    triangles[t++] = new XTriangle
                    {
                        p1 = new XPointFixed {x = (p2.X << 16) + ox, y = (p2.Y << 16) + oy},
                        p2 = new XPointFixed {x = (p1.X << 16) - ox, y = (p1.Y << 16) - oy},
                        p3 = new XPointFixed {x = (p2.X << 16) - ox, y = (p2.Y << 16) - oy}
                    };
                }

                LibXRender.XRenderCompositeTriangles(display, PictOp.PictOpOver, brush, pictureId, pictFormatPtr, 0, 0, triangles, triangles.Length);
            }
            finally
            {
                LibXRender.XRenderFreePicture(display, brush);
            }
        }

        private void DrawPathWithXRenderCompositeDoublePoly(Color color, int width, Point[] points)
        {
            if (width == 0)
            {
                width = 1;
            }

            XRenderColor xColor = new XRenderColor(color);
            // todo: cache brush
            ulong brush = LibXRender.XRenderCreateSolidFill(display, ref xColor);
            try
            {
                XPointDouble[] polyPoints = new XPointDouble[points.Length * 2];
                for (int i = 1, t = polyPoints.Length - 2; i < points.Length; i++, t--)
                {
                    Point p1 = points[i - 1];
                    Point p2 = points[i];

                    p1.Offset(-origin.X, -origin.Y);
                    p2.Offset(-origin.X, -origin.Y);

                    int dx = p2.X - p1.X;
                    int dy = p2.Y - p1.Y;
                    double len = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    double ox = -dy / len * width / 2;
                    double oy = dx / len * width / 2;

                    polyPoints[i] = new XPointDouble {x = points[i].X + ox, y = points[i].Y + oy};
                    polyPoints[t] = new XPointDouble {x = points[i].X - ox, y = points[i].Y - oy};

                    if (i == 1)
                    {
                        // todo: handle separately without if
                        polyPoints[0] = new XPointDouble {x = points[0].X + ox, y = points[0].Y + oy};
                        polyPoints[polyPoints.Length - 1] = new XPointDouble {x = points[0].X - ox, y = points[0].Y - oy};
                    }
                }

                LibXRender.XRenderCompositeDoublePoly(display, PictOp.PictOpOver, brush, pictureId, pictFormatPtr, 0, 0, 0, 0, polyPoints, polyPoints.Length, 1);
            }
            finally
            {
                LibXRender.XRenderFreePicture(display, brush);
            }
        }

        private void DrawPathWithXRenderCompositeTriStrip(Color color, int width, Point[] points)
        {
            if (width == 0)
            {
                width = 1;
            }

            XRenderColor xColor = new XRenderColor(color);
            // todo: cache brush
            ulong brush = LibXRender.XRenderCreateSolidFill(display, ref xColor);
            try
            {
                XPointFixed[] stripPoints = new XPointFixed[(points.Length - 1) * 3 + 1];
                for (int i = 1, t = 0, m = 1; i < points.Length; i++, m = -m)
                {
                    Point p1 = points[i - 1];
                    Point p2 = points[i];

                    p1.Offset(-origin.X, -origin.Y);
                    p2.Offset(-origin.X, -origin.Y);

                    int dx = p2.X - p1.X;
                    int dy = p2.Y - p1.Y;
                    double len = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    // todo: handle 0 points and zero-length segments
                    int ox = Convert.ToInt32(-dy / len * 32768 * width) * m;
                    int oy = Convert.ToInt32(dx / len * 32768 * width) * m;

                    if (i == 1)
                    {
                        // todo: handle separately without if
                        stripPoints[t++] = new XPointFixed {x = (p1.X << 16) + ox, y = (p1.Y << 16) + oy};
                    }

                    stripPoints[t++] = new XPointFixed {x = (p1.X << 16) - ox, y = (p1.Y << 16) - oy};
                    stripPoints[t++] = new XPointFixed {x = (p2.X << 16) + ox, y = (p2.Y << 16) + oy};
                    stripPoints[t++] = new XPointFixed {x = (p2.X << 16) - ox, y = (p2.Y << 16) - oy};
                }

                LibXRender.XRenderCompositeTriStrip(display, PictOp.PictOpOver, brush, pictureId, pictFormatPtr, 0, 0, stripPoints, stripPoints.Length);
            }
            finally
            {
                LibXRender.XRenderFreePicture(display, brush);
            }
        }

        public void FillEllipse(Color color, int x, int y, int width, int height)
        {
            x -= origin.X;
            y -= origin.Y;

            if (color.IsFullyOpaque())
            {
                FillEllipseXLib(color, x, y, width, height);
                return;
            }

            WithExtraCanvas
            (
                new Rectangle(x, y, width, height),
                canvas => canvas.FillEllipseXLib(color, 0, 0, width, height)
            );
        }

        private void FillEllipseXLib(Color color, int x, int y, int width, int height)
        {
            var gcValues = new XGCValues();
            gcValues.foreground = ToPArgb(color);

            var gcValueMask = XGCValueMask.GCForeground;

            var gc = LibX11.XCreateGC(display, drawableId, gcValueMask, ref gcValues);
            try
            {
                if (clipRectangles != null)
                {
                    LibX11.XSetClipRectangles(display, gc, 0, 0, clipRectangles, clipRectangles.Length, 0);
                }

                LibX11.XDrawArc(display, drawableId, gc, x, y, (uint) width - 1, (uint) height - 1, 0, 360 * 64);
                LibX11.XFillArc(display, drawableId, gc, x, y, (uint) width - 1, (uint) height - 1, 0, 360 * 64);
            }
            finally
            {
                LibX11.XFreeGC(display, gc);
            }
        }

        private void WithExtraCanvas(Rectangle rect, Action<X11Canvas> action)
        {
            using (X11Image tempImage = X11Image.Create(display, visual, drawableId, rect.Width, rect.Height))
            {
                var gcValues = new XGCValues();
                gcValues.foreground = ToPArgb(Color.Transparent);
                var gcValueMask = XGCValueMask.GCForeground;
                var gc = LibX11.XCreateGC(display, drawableId, gcValueMask, ref gcValues);
                try
                {
                    // todo: check int -> uint conversion
                    LibX11.XFillRectangle(display, tempImage.PixmapId, gc, 0, 0, (uint) rect.Width, (uint) rect.Height);
                }
                finally
                {
                    LibX11.XFreeGC(display, gc);
                }

                using (var innerCanvas = X11Canvas.CreateForDrawable(display, screenNum, objectCache, visual, colormap, pictFormatPtr, tempImage.PixmapId))
                {
                    action(innerCanvas);
                    DrawImage(tempImage, rect.X + origin.X, rect.Y + origin.Y);
                }
            }
        }

        private static ulong ToPArgb(Color color)
        {
            int a = color.A;
            int r = color.R * a / 255;
            int g = color.G * a / 255;
            int b = color.B * a / 255;
            return (ulong) (a << 24 | r << 16 | g << 8 | b);
        }
    }
}