using System.IO;

namespace NWindows.NativeApi
{
    internal interface INativeImageCodec
    {
        INativeImage LoadImageFromStream(Stream stream);
    }
}