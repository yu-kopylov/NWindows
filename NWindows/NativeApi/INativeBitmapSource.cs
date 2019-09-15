using System;
using System.Drawing;

namespace NWindows.NativeApi
{
    internal interface INativeBitmapSource
    {
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// <para>Copies content of this image source into a bitmap.</para>
        /// <para>Bitmap format is 32 bits per pixel ARGB with straight alpha in native byte order.</para>
        /// </summary>
        /// <param name="imageArea">Area of the image that should be copied.</param>
        /// <param name="bitmap">Pointer to the destination bitmap.</param>
        /// <param name="bitmapStride">Number of bytes between start of one row of the pixels to start of the next row.</param>
        /// <exception cref="NullReferenceException">If <paramref name="bitmap"/> is <see cref="IntPtr.Zero"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// <para>When the given <paramref name="imageArea"/> does not fit within the area of this image source.</para>
        /// <para>When width or height of the given <paramref name="imageArea"/> is negative.</para>
        /// <para>When <paramref name="bitmapStride"/> is negative.</para>
        /// <para>When <paramref name="bitmapStride"/> is less than 4 * (width of <paramref name="imageArea"/>).</para>
        /// <para>When required size of target buffer exceeds <see cref="int.MaxValue"/>.</para>
        /// </exception>
        void CopyToBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride);
    }

    internal static class NativeBitmapSourceParameterValidation
    {
        /// <summary>
        /// Checks parameters of <see cref="INativeBitmapSource.CopyToBitmap"/>.
        /// </summary>
        public static void CopyToBitmap(INativeBitmapSource source, Rectangle sourceArea, IntPtr bitmap, int stride, out int requiredBufferSize)
        {
            if (bitmap == IntPtr.Zero)
            {
                throw new NullReferenceException($"Pointer to bitmap cannot be {nameof(IntPtr.Zero)}.");
            }

            if (sourceArea.Left < 0 || sourceArea.Top < 0 || sourceArea.Right > source.Width || sourceArea.Bottom > source.Height)
            {
                throw new InvalidOperationException
                (
                    $"Source area ({sourceArea}) does not fit within the area of this source ({new Rectangle(0, 0, source.Width, source.Height)})."
                );
            }

            if (sourceArea.Width < 0)
            {
                throw new InvalidOperationException($"Width of source area ({sourceArea.Width}) cannot be negative.");
            }

            if (sourceArea.Height < 0)
            {
                throw new InvalidOperationException($"Height of source area ({sourceArea.Height}) cannot be negative.");
            }

            if (stride < 0)
            {
                throw new InvalidOperationException($"Stride ({stride}) cannot be negative.");
            }

            if (stride < 4L * sourceArea.Width)
            {
                throw new InvalidOperationException($"Stride ({stride}) is less than 4 * (width of source area ({sourceArea.Width})).");
            }

            if (sourceArea.Height == 0)
            {
                requiredBufferSize = 0;
            }
            else
            {
                long requiredBufferSizeL = (sourceArea.Height - 1L) * stride + sourceArea.Width * 4L;
                if (requiredBufferSizeL > int.MaxValue)
                {
                    throw new InvalidOperationException($"Required size of target buffer ({requiredBufferSizeL}) exceeds {int.MaxValue}.");
                }

                requiredBufferSize = (int) requiredBufferSizeL;
            }
        }
    }
}