using System;
using System.Drawing;
using NWindows.NativeApi;
using NWindows.Win32;

namespace NWindows.X11
{
    internal class X11Image : INativeImage
    {
        public int Width { get; }
        public int Height { get; }
        public IntPtr XImage { get; }
        public IntPtr ImageData { get; }

        public X11Image(int width, int height, IntPtr xImage, IntPtr imageData)
        {
            XImage = xImage;
            ImageData = imageData;
            Width = width;
            Height = height;
        }

        public void Dispose()
        {
            // todo: use finalizer?
            LibX11.XDestroyImage(XImage);
        }

        public void CopyFromBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
        {
            // todo: create separate validation ?
            NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

            IntPtr pixelsPtr = ImageData + (imageArea.Y * Width + imageArea.X) * 4;
            PixelConverter.Convert_ARGB_32_To_PARGB_32(bitmap, bitmapStride, pixelsPtr, Width * 4, imageArea.Width, imageArea.Height);
        }

        public void CopyToBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
        {
            NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

            IntPtr pixelsPtr = ImageData + (imageArea.Y * Width + imageArea.X) * 4;
            PixelConverter.Convert_PARGB_32_To_ARGB_32(pixelsPtr, Width * 4, bitmap, bitmapStride, imageArea.Width, imageArea.Height);
        }
    }
}