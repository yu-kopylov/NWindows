using System;

namespace NWindows.X11
{
    // todo: make IImage disposable?
    internal class X11Image : IImage, IDisposable
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
            LibX11.XDestroyImage(XImage);
        }
    }
}