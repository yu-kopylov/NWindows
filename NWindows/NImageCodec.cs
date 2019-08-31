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
            INativeImage nativeImage = NativeCodec.LoadFromFile(filename);
            return new NImage(this, nativeImage);
        }
    }
}