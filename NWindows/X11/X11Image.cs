using System;
using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Image : INativeImage
    {
        public IntPtr Display { get; }
        public IntPtr Visual { get; }
        public ulong PixmapId { get; }
        public int Width { get; }
        public int Height { get; }

        private X11Image(IntPtr display, IntPtr visual, ulong pixmapId, int width, int height)
        {
            Display = display;
            Visual = visual;
            PixmapId = pixmapId;
            Width = width;
            Height = height;
        }

        public void Dispose()
        {
            // todo: use finalizer?
            LibX11.XFreePixmap(Display, PixmapId);
        }

        public static X11Image Create(IntPtr display, IntPtr visual, ulong drawable, int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException($"Image dimensions cannot be negative ({width} x {height}).");
            }

            // todo: handle zero width / height

            ulong pixmapId = LibX11.XCreatePixmap(
                display,
                drawable,
                (uint) width,
                (uint) height,
                X11Application.RequiredColorDepth
            );

            return new X11Image(display, visual, pixmapId, width, height);
        }

        public void CopyFromBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
        {
            // todo: create separate validation ?
            NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

            using (X11Bitmap xBitmap = X11Bitmap.Create(Display, Visual, imageArea.Width, imageArea.Height))
            {
                PixelConverter.Convert_ARGB_32_To_PARGB_32(bitmap, bitmapStride, xBitmap.ImageData, imageArea.Width * 4, imageArea.Width, imageArea.Height);

                var gcValues = new XGCValues();
                var gc = LibX11.XCreateGC(Display, PixmapId, 0, ref gcValues);
                try
                {
                    LibX11.XPutImage(Display, PixmapId, gc, xBitmap.XImage, 0, 0, imageArea.X, imageArea.Y, (uint) imageArea.Width, (uint) imageArea.Height);
                }
                finally
                {
                    LibX11.XFreeGC(Display, gc);
                }
            }
        }

        public void CopyToBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
        {
            NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

            using (X11Bitmap xBitmap = X11Bitmap.Create(Display, Visual, imageArea.Width, imageArea.Height))
            {
                LibX11.XGetSubImage(
                    Display,
                    PixmapId,
                    imageArea.X,
                    imageArea.Y,
                    (uint) imageArea.Width,
                    (uint) imageArea.Height,
                    ulong.MaxValue,
                    XImageFormat.ZPixmap,
                    xBitmap.XImage,
                    0,
                    0
                );

                PixelConverter.Convert_PARGB_32_To_ARGB_32(xBitmap.ImageData, imageArea.Width * 4, bitmap, bitmapStride, imageArea.Width, imageArea.Height);
            }
        }
    }
}