using System;
using System.Drawing;
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

        public NBitmap LoadBitmapFromFile(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return LoadBitmapFromStream(stream);
            }
        }

        public NBitmap LoadBitmapFromStream(Stream stream)
        {
            return nativeCodec.LoadBitmapFromStream(stream, source =>
            {
                NBitmap bitmap = new NBitmap(source.Width, source.Height);
                bitmap.WithPinnedPixels(ptr => source.CopyToBitmap(new Rectangle(0, 0, source.Width, source.Height), ptr, 4 * source.Width));
                return bitmap;
            });
        }

        public NImage Create(int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException($"Image dimensions cannot be negative ({width} x {height}).");
            }

            if (width * 4L > int.MaxValue || height * 4L > int.MaxValue || 4L * width * height > int.MaxValue)
            {
                throw new ArgumentException($"Image dimensions are too large ({width} x {height}).");
            }

            return new NImage(this, nativeCodec.CreateImage(width, height));
        }
    }
}