namespace NWindows.NativeApi
{
    internal interface INativeImageCodec
    {
        INativeImage LoadFromFile(string filename);
    }
}