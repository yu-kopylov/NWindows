using System;

namespace NWindows.NativeApi
{
    internal interface INativeApplication : IDisposable
    {
        void Run(INativeWindowStartupInfo startupInfo);

        INativeGraphics Graphics { get; }
        INativeImageCodec ImageCodec { get; }
        INativeClipboard Clipboard { get; }
    }
}