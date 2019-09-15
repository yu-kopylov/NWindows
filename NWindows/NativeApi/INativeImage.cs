using System;
using System.Drawing;

namespace NWindows.NativeApi
{
    // todo: should this interface include IDisposable?
    internal interface INativeImage : IDisposable, INativeBitmapSource
    {
        int Width { get; }
        int Height { get; }

        void CopyFromBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride);
    }
}