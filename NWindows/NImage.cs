using System;
using System.Drawing;
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

        private NImageCodec Codec { get; }

        internal INativeImage NativeImage { get; }

        public int Width => NativeImage.Width;

        public int Height => NativeImage.Height;

        public void CopyFromBitmap(NBitmap source, Point sourceLocation, Point destLocation, Size size)
        {
            // todo: check other conditions and and test validation

            if (size.Width <= 0 || size.Height <= 0)
            {
                // nothing to copy
                return;
            }

            if (sourceLocation.X < 0 || sourceLocation.Y < 0 || sourceLocation.X + size.Width > source.Width || sourceLocation.Y + size.Height > source.Width)
            {
                throw new ArgumentException($"Source area is outside of the bitmap: ({new Rectangle(sourceLocation, size)}).");
            }

            source.WithPinnedPixels(sourcePtr =>
            {
                int bitmapOffset = (sourceLocation.Y * source.Width + sourceLocation.X) * 4;
                NativeImage.CopyFromBitmap(new Rectangle(destLocation, size), sourcePtr + bitmapOffset, source.Width * 4);
            });
        }

        public void CopyToBitmap(Point sourceLocation, NBitmap dest, Point destLocation, Size size)
        {
            // todo: check other conditions and and test validation

            if (size.Width <= 0 || size.Height <= 0)
            {
                // nothing to copy
                return;
            }

            if (sourceLocation.X < 0 || sourceLocation.Y < 0 || sourceLocation.X + size.Width > Width || sourceLocation.Y + size.Height > Width)
            {
                throw new ArgumentException($"Source area is outside of the image: ({new Rectangle(sourceLocation, size)}).");
            }

            dest.WithPinnedPixels(destPtr =>
            {
                int bitmapOffset = (destLocation.Y * dest.Width + destLocation.X) * 4;
                NativeImage.CopyToBitmap(new Rectangle(sourceLocation, size), destPtr + bitmapOffset, dest.Width * 4);
            });
        }
    }
}