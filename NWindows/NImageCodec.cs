using System.IO;
using NWindows.NativeApi;

namespace NWindows
{
    public class NImageCodec
    {
        private readonly INativeImageCodec nativeCodec;

        internal NImageCodec(INativeImageCodec nativeCodec)
        {
            this.nativeCodec = nativeCodec;
        }

        public NImage LoadImageFromFile(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return LoadImageFromStream(stream);
            }
        }

        public NImage LoadImageFromStream(Stream stream)
        {
            INativeImage nativeImage = nativeCodec.LoadImageFromStream(stream);
            return new NImage(this, nativeImage);
        }
    }
}