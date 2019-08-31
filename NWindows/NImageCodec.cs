using System.IO;
using NWindows.NativeApi;

namespace NWindows
{
    public class NImageCodec
    {
        internal NImageCodec(INativeImageCodec nativeCodec)
        {
            NativeCodec = nativeCodec;
        }

        internal INativeImageCodec NativeCodec { get; }

        public NImage LoadImageFromFile(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return LoadImageFromStream(stream);
            }
        }

        public NImage LoadImageFromStream(Stream stream)
        {
            INativeImage nativeImage = NativeCodec.LoadImageFromStream(stream);
            return new NImage(this, nativeImage);
        }
    }
}