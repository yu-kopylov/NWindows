using System;
using NWindows.NativeApi;

namespace NWindows
{
    /// <summary>
    /// <para>Represents a platform-dependent image suitable for rendering on canvas.</para>
    /// <para>Image format usually uses premultiplied alpha.</para>
    /// </summary>
    public class NImage : IDisposable
    {
        internal NImage(NImageCodec codec, INativeImage nativeImage)
        {
            Codec = codec;
            NativeImage = nativeImage;
        }

        public void Dispose()
        {
            NativeImage.Dispose();
        }

        internal NImageCodec Codec { get; }

        internal INativeImage NativeImage { get; }

        public int Width => NativeImage.Width;

        public int Height => NativeImage.Height;
    }
}