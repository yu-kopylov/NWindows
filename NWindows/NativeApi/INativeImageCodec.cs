using System.IO;

namespace NWindows.NativeApi
{
    internal delegate T CreateBitmapDelegate<T>(INativeBitmapSource source);

    internal interface INativeImageCodec
    {
        INativeImage LoadImageFromStream(Stream stream);
        T LoadBitmapFromStream<T>(Stream stream, CreateBitmapDelegate<T> create);
    }
}