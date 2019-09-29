namespace NWindows.NativeApi
{
    internal interface INativeApplication
    {
        void Init();
        void Run(INativeWindowStartupInfo window);

        INativeGraphics CreateGraphics();
        INativeImageCodec CreateImageCodec();
    }
}