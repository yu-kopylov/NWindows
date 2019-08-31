using System;

namespace NWindows.NativeApi
{
    // todo: should this interface include IDisposable?
    internal interface INativeImage : IDisposable
    {
        int Width { get; }
        int Height { get; }
    }
}