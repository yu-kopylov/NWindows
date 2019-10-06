using System;
using System.Runtime.InteropServices;

namespace NWindows.X11
{
    internal class X11Bitmap : IDisposable
    {
        public IntPtr XImage { get; }
        public IntPtr ImageData { get; }

        private X11Bitmap(IntPtr xImage, IntPtr imageData)
        {
            XImage = xImage;
            ImageData = imageData;
        }

        public void Dispose()
        {
            // todo: use finalizer?
            LibX11.XDestroyImage(XImage);
        }

        public static X11Bitmap Create(IntPtr display, IntPtr visual, int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException($"Image dimensions cannot be negative ({width} x {height}).");
            }

            IntPtr imageData = Marshal.AllocHGlobal(4 * width * height);
            IntPtr xImage = IntPtr.Zero;

            try
            {
                xImage = LibX11.XCreateImage
                (
                    display,
                    visual,
                    X11Application.RequiredColorDepth,
                    XImageFormat.ZPixmap,
                    0,
                    imageData,
                    (uint) width,
                    (uint) height,
                    X11Application.RequiredColorDepth,
                    width * 4
                );

                X11Bitmap bitmap = new X11Bitmap(xImage, imageData);

                xImage = IntPtr.Zero;
                imageData = IntPtr.Zero;

                return bitmap;
            }
            finally
            {
                if (xImage != IntPtr.Zero)
                {
                    LibX11.XDestroyImage(xImage);
                }
                else if (imageData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(imageData);
                }
            }
        }
    }
}