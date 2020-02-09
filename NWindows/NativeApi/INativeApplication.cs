using System;

namespace NWindows.NativeApi
{
    internal interface INativeApplication : IDisposable
    {
        void Init();
        void Run(INativeWindowStartupInfo startupInfo);

        INativeGraphics CreateGraphics();
        INativeImageCodec CreateImageCodec();
        INativeClipboard CreateClipboard();
    }
}