namespace NWindows.NativeApi
{
    internal interface INativeApplication
    {
        void Init();
        void Run(INativeWindowStartupInfo startupInfo);

        INativeGraphics CreateGraphics();
        INativeImageCodec CreateImageCodec();
        INativeClipboard CreateClipboard();
    }
}