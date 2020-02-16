using System;
using System.Drawing;

namespace NWindows.Win32
{
    internal class Win32Bitmap : IDisposable
    {
        private readonly IntPtr bitmapPtr;
        private readonly IntPtr bitmapHdc;
        private readonly int width;
        private readonly int height;

        private Win32Bitmap(IntPtr bitmapPtr, IntPtr bitmapHdc, int width, int height)
        {
            this.bitmapPtr = bitmapPtr;
            this.bitmapHdc = bitmapHdc;
            this.width = width;
            this.height = height;
        }

        public void Dispose()
        {
            Gdi32API.DeleteDC(bitmapHdc);
            Gdi32API.DeleteObject(bitmapPtr);
        }

        public static Win32Bitmap Create(IntPtr hdc, int width, int height)
        {
            IntPtr bitmapPtr = IntPtr.Zero;
            IntPtr bitmapHdc = IntPtr.Zero;
            try
            {
                bitmapPtr = Gdi32API.CreateCompatibleBitmapChecked(hdc, width, height);
                bitmapHdc = Gdi32API.CreateCompatibleDCChecked(hdc);
                Gdi32API.SelectObjectChecked(bitmapHdc, bitmapPtr);

                var bitmap = new Win32Bitmap(bitmapPtr, bitmapHdc, width, height);
                bitmapPtr = IntPtr.Zero;
                bitmapHdc = IntPtr.Zero;
                return bitmap;
            }
            finally
            {
                if (bitmapHdc != IntPtr.Zero)
                {
                    Gdi32API.DeleteDC(bitmapHdc);
                }

                if (bitmapPtr != IntPtr.Zero)
                {
                    Gdi32API.DeleteObject(bitmapPtr);
                }
            }
        }

        public Win32Canvas CreateCanvas(Point offset, Gdi32ObjectCache objectCache)
        {
            return new Win32Canvas(bitmapHdc, offset, objectCache);
        }

        public void CopyTo(IntPtr hdc, int x, int y)
        {
            Gdi32API.BitBlt(hdc, x, y, width, height, bitmapHdc, 0, 0, GDI32RasterOperation.SRCCOPY);
        }
    }
}