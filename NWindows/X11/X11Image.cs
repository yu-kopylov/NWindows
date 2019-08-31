using System;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Image : INativeImage
    {
        public IntPtr XImage { get; }
        public int Width { get; }
        public int Height { get; }

        public X11Image(IntPtr xImage, int width, int height)
        {
            XImage = xImage;
            Width = width;
            Height = height;
        }

        public void Dispose()
        {
            // todo: use finalizer?
            LibX11.XDestroyImage(XImage);
        }
    }
}