using System;
using System.Drawing;
using System.Runtime.InteropServices;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class W32Image : INativeImage
    {
        public W32Image(int width, int height, byte[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public void Dispose()
        {
            // todo: dispose?
        }

        public int Width { get; }

        public int Height { get; }

        internal byte[] Pixels { get; }

        public void CopyFromBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
        {
            // todo: create separate validation ?
            NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

            GCHandle pixelsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);
            try
            {
                IntPtr pixelsPtr = pixelsHandle.AddrOfPinnedObject() + (imageArea.Y * Width + imageArea.X) * 4;
                PixelConverter.ConvertArgb32ToPArgb32(bitmap, bitmapStride, pixelsPtr, Width * 4, imageArea.Width, imageArea.Height);
            }
            finally
            {
                pixelsHandle.Free();
            }
        }

        public void CopyToBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
        {
            //todo: rename area parameter in both CopyToBitmap and CopyFromBitmap

            NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

            GCHandle pixelsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);
            try
            {
                IntPtr pixelsPtr = pixelsHandle.AddrOfPinnedObject() + (imageArea.Y * Width + imageArea.X) * 4;
                PixelConverter.ConvertPArgb32ToArgb32(pixelsPtr, Width * 4, bitmap, bitmapStride, imageArea.Width, imageArea.Height);
            }
            finally
            {
                pixelsHandle.Free();
            }
        }
    }
}