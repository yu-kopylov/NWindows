﻿using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32Canvas : INativeCanvas, IDisposable
    {
        private readonly IntPtr hdc;
        private readonly Point offset;

        private IntPtr graphics;
        private IntPtr defaultStringFormat;

        private readonly Gdi32ObjectCache objectCache;

        private IntPtr originalPen;
        private IntPtr originalBrush;
        private IntPtr originalFont;

        public Win32Canvas(IntPtr hdc, Point offset, Gdi32ObjectCache objectCache)
        {
            this.hdc = hdc;
            this.offset = offset;
            this.objectCache = objectCache;
        }

        public void Dispose()
        {
            if (graphics != IntPtr.Zero)
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteGraphics(graphics));
                graphics = IntPtr.Zero;
            }

            if (defaultStringFormat != IntPtr.Zero)
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteStringFormat(defaultStringFormat));
                graphics = IntPtr.Zero;
            }

            if (originalPen != IntPtr.Zero)
            {
                Gdi32API.SelectObjectChecked(hdc, originalPen);
            }

            if (originalBrush != IntPtr.Zero)
            {
                Gdi32API.SelectObjectChecked(hdc, originalBrush);
            }

            if (originalFont != IntPtr.Zero)
            {
                Gdi32API.SelectObjectChecked(hdc, originalFont);
            }
        }

        private void PrepareGraphics()
        {
            if (graphics == IntPtr.Zero)
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateFromHDC(hdc, out graphics));
            }
        }

        private void PrepareDefaultStringFormat()
        {
            if (defaultStringFormat == IntPtr.Zero)
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipStringFormatGetGenericDefault(out defaultStringFormat));
            }
        }

        public void SetClipRectangle(int x, int y, int width, int height)
        {
            x += offset.X;
            y += offset.Y;

            var region = Gdi32API.CreateRectRgnChecked(x, y, x + width, y + height);
            try
            {
                Gdi32API.SelectClipRgn(hdc, region);
            }
            finally
            {
                Gdi32API.DeleteObject(region);
            }
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            x += offset.X;
            y += offset.Y;

            // todo: choose W32/GDI/GDI+ method
            //            FillRectangleW32(color, x, y, width, height);
            FillRectangleGDI(color, x, y, width, height);
            //            FillRectangleGDIPlus(color, x, y, width, height);
        }

        public void DrawString(Color color, FontConfig font, int x, int y, string text)
        {
            x += offset.X;
            y += offset.Y;

            // todo: pick
            DrawStringGDI(color, font, x, y, text);
            // DrawStringGDIPlus(color, font, x, y, text);
        }

        public void DrawImage(INativeImage image, int x, int y)
        {
            x += offset.X;
            y += offset.Y;

            // todo: allow null?
            Win32Image win32Image = (Win32Image) image;

            var bitmap = Gdi32API.CreateDIBSectionChecked(hdc, new BITMAPINFO(win32Image.Width, win32Image.Height), out IntPtr buffer);
            IntPtr hdcMem = IntPtr.Zero;
            try
            {
                Marshal.Copy(win32Image.Pixels, 0, buffer, win32Image.Pixels.Length);
                hdcMem = Gdi32API.CreateCompatibleDCChecked(hdc);
                var oldBitmap = Gdi32API.SelectObjectChecked(hdcMem, bitmap);

                Gdi32API.GdiAlphaBlend
                (
                    hdc,
                    x, y, win32Image.Width, win32Image.Height,
                    hdcMem,
                    0, 0, win32Image.Width, win32Image.Height,
                    BLENDFUNCTION.SourceAlpha()
                );

                Gdi32API.SelectObjectChecked(hdcMem, oldBitmap);
            }
            finally
            {
                Gdi32API.SafeDeleteObject(hdcMem);
                Gdi32API.DeleteObject(bitmap);
            }
        }

        public void DrawPath(Color color, int width, Point[] points)
        {
            // todo: handle points.Length <= 1

            var relativePoints = new Point[points.Length];
            for (int i = 0; i < relativePoints.Length; i++)
            {
                var point = points[i];
                relativePoints[i] = new Point(point.X + offset.X, point.Y + offset.Y);
            }

            if (color.IsFullyOpaque())
            {
                DrawOpaquePathGDI(color, width, relativePoints);
                return;
            }

            int minX = relativePoints.Min(p => p.X) - width;
            int minY = relativePoints.Min(p => p.Y) - width;
            int maxX = relativePoints.Max(p => p.X) + width;
            int maxY = relativePoints.Max(p => p.Y) + width;

            for (int i = 0; i < relativePoints.Length; i++)
            {
                var point = relativePoints[i];
                relativePoints[i] = new Point(point.X - minX, point.Y - minY);
            }

            WithTransparentCanvas
            (
                new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1),
                color.A, true,
                canvas => canvas.DrawOpaquePathGDI(color, width, relativePoints)
            );
        }

        private void DrawOpaquePathGDI(Color color, int width, Point[] points)
        {
            SelectSolidPen(color, width);
            // todo: unlike X11, GDI does not paint last pixel
            Gdi32API.Polyline(hdc, points.Select(p => new POINT(p.X, p.Y)).ToArray(), points.Length);
        }

        public void FillEllipse(Color color, int x, int y, int width, int height)
        {
            x += offset.X;
            y += offset.Y;

            if (color.IsFullyOpaque())
            {
                FillOpaqueEllipseGDI(color, x, y, width, height);
                return;
            }

            WithTransparentCanvas
            (
                new Rectangle(x, y, width, height),
                color.A, true,
                canvas => canvas.FillOpaqueEllipseGDI(color, 0, 0, width, height)
            );
        }

        public void FillOpaqueEllipseGDI(Color color, int x, int y, int width, int height)
        {
            SelectSolidPen(color, 0);
            SelectSolidBrush(color);
            Gdi32API.Ellipse(hdc, x, y, x + width, y + height);
        }

        private void DrawStringGDI(Color color, FontConfig font, int x, int y, string text)
        {
            if (color.IsFullyOpaque())
            {
                DrawOpaqueStringGDI(color, font, x, y, text);
                return;
            }

            SelectGDIFont(font);
            Gdi32API.GetTextExtentPoint32W(hdc, text, text.Length, out var size);

            int leftPadding, rightPadding;
            if (font.IsItalic)
            {
                int infFontSize = Convert.ToInt32(font.Size);
                leftPadding = 1 + (4 * infFontSize + 199) / 200;
                rightPadding = 1 + (11 * infFontSize + 199) / 200;
            }
            else
            {
                leftPadding = 0;
                rightPadding = 0;
            }

            WithTransparentCanvas(
                new Rectangle(x - leftPadding, y, size.cx + leftPadding + rightPadding, size.cy),
                color.A, true,
                canvas => canvas.DrawOpaqueStringGDI(color, font, leftPadding, 0, text)
            );
        }

        private void DrawOpaqueStringGDI(Color color, FontConfig font, int x, int y, string text)
        {
            SelectGDIFont(font);
            Gdi32API.SetBkModeChecked(hdc, Gdi32BackgroundMode.TRANSPARENT);
            Gdi32API.SetTextColorChecked(hdc, Gdi32API.ToCOLORREF(color));
            Gdi32API.TextOutW(hdc, x, y, text, text.Length);
        }

        private void DrawStringGDIPlus(Color color, FontConfig font, int x, int y, string text)
        {
            PrepareGraphics();
            PrepareDefaultStringFormat();
            GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateFontFamilyFromName(font.FontFamily, IntPtr.Zero, out var fontFamilyPtr));
            try
            {
                FontStyle fontStyle = GdiPlusAPI.GetFontStyle(font);
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateFont(fontFamilyPtr, font.Size, fontStyle, Unit.UnitPixel, out var fontPtr));
                try
                {
                    GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateSolidFill(color.ToArgb(), out var brush));
                    try
                    {
                        RectF rect = new RectF(x, y, 0, 0);
                        GdiPlusAPI.GdipDrawString(graphics, text, text.Length, fontPtr, ref rect, defaultStringFormat, brush);
                    }
                    finally
                    {
                        GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteBrush(brush));
                    }
                }
                finally
                {
                    GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteFont(fontPtr));
                }
            }
            finally
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteFontFamily(fontFamilyPtr));
            }
        }

        private void FillRectangleW32(Color color, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            if (color.IsFullyOpaque())
            {
                FillOpaqueRectangleW32(color, x, y, width, height);
                return;
            }

            WithTransparentCanvas
            (
                new Rectangle(x, y, width, height),
                color.A, false,
                canvas => canvas.FillOpaqueRectangleW32(color, 0, 0, width, height)
            );
        }

        private void FillOpaqueRectangleW32(Color color, int x, int y, int width, int height)
        {
            // todo: check that width and height are exact
            RECT rect = new RECT {left = x, top = y, right = x + width, bottom = y + height};
            var brush = objectCache.GetSolidBrush(color);
            Win32API.FillRect(hdc, ref rect, brush);
        }

        private void FillRectangleGDI(Color color, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            if (color.IsFullyOpaque())
            {
                FillOpaqueRectangleGDI(color, x, y, width, height);
                return;
            }

            WithTransparentCanvas
            (
                new Rectangle(x, y, width, height),
                color.A, false,
                canvas => canvas.FillOpaqueRectangleGDI(color, 0, 0, width, height)
            );
        }

        private void FillOpaqueRectangleGDI(Color color, int x, int y, int width, int height)
        {
            // todo: check that width and height are exact
            var region = Gdi32API.CreateRectRgnChecked(x, y, x + width, y + height);
            try
            {
                // todo: pick FillRgn or PaintRgn
                Gdi32API.FillRgn(hdc, region, objectCache.GetSolidBrush(color));

                // SelectSolidBrush(color);
                // Gdi32API.PaintRgn(hdc, region);
            }
            finally
            {
                Gdi32API.DeleteObject(region);
            }
        }

        private void FillRectangleGDIPlus(Color color, int x, int y, int width, int height)
        {
            PrepareGraphics();
            GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateSolidFill(color.ToArgb(), out var brush));
            try
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipFillRectangleI(graphics, brush, x, y, width, height));
            }
            finally
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteBrush(brush));
            }
        }

        private void WithTransparentCanvas(Rectangle rect, byte alpha, bool copySource, Action<Win32Canvas> action)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || alpha == 0)
            {
                return;
            }

            IntPtr memoryHdc = Gdi32API.CreateCompatibleDCChecked(hdc);
            try
            {
                IntPtr memoryBitmap = Gdi32API.CreateCompatibleBitmapChecked(hdc, rect.Width, rect.Height);
                try
                {
                    var originalBitmap = Gdi32API.SelectObjectChecked(memoryHdc, memoryBitmap);
                    try
                    {
                        if (copySource)
                        {
                            Gdi32API.BitBlt(memoryHdc, 0, 0, rect.Width, rect.Height, hdc, rect.X, rect.Y, GDI32RasterOperation.SRCCOPY);
                        }

                        using (Win32Canvas memoryCanvas = new Win32Canvas(memoryHdc, offset, objectCache))
                        {
                            action(memoryCanvas);
                        }

                        Gdi32API.GdiAlphaBlend
                        (
                            hdc,
                            rect.X, rect.Y, rect.Width, rect.Height,
                            memoryHdc,
                            0, 0, rect.Width, rect.Height,
                            BLENDFUNCTION.ConstantAlpha(alpha)
                        );
                    }
                    finally
                    {
                        Gdi32API.SelectObjectChecked(memoryHdc, originalBitmap);
                    }
                }
                finally
                {
                    Gdi32API.DeleteObject(memoryBitmap);
                }
            }
            finally
            {
                Gdi32API.DeleteDC(memoryHdc);
            }
        }

        private void SelectSolidPen(Color color, int width)
        {
            IntPtr pen = objectCache.GetSolidPen(color, width);
            IntPtr oldPen = Gdi32API.SelectObjectChecked(hdc, pen);
            if (originalPen == IntPtr.Zero)
            {
                originalPen = oldPen;
            }
        }

        private void SelectSolidBrush(Color color)
        {
            IntPtr brush = objectCache.GetSolidBrush(color);
            IntPtr oldBrush = Gdi32API.SelectObjectChecked(hdc, brush);
            if (originalBrush == IntPtr.Zero)
            {
                originalBrush = oldBrush;
            }
        }

        private void SelectGDIFont(FontConfig fontConfig)
        {
            IntPtr font = objectCache.GetFont(fontConfig);
            IntPtr oldFont = Gdi32API.SelectObjectChecked(hdc, font);
            if (originalFont == IntPtr.Zero)
            {
                originalFont = oldFont;
            }
        }
    }
}