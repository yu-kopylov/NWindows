using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NWindows
{
    /// <summary>
    /// <para>Represents a platform-independent image suitable for direct access to pixels by native and managed libraries.</para>
    /// <para>Image format is 32 bits per pixel ARGB with straight alpha in native byte-order.</para>
    /// </summary>
    public class NBitmap
    {
        private readonly Color32[,] pixels;

        public int Width { get; }
        public int Height { get; }

        public NBitmap(int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException($"Bitmap dimensions cannot be negative ({width} x {height}).");
            }

            if (width * 4L > int.MaxValue || height * 4L > int.MaxValue || 4L * width * height > int.MaxValue)
            {
                throw new ArgumentException($"Bitmap dimensions are too large ({width} x {height}).");
            }

            Width = width;
            Height = height;
            pixels = new Color32[height, width];
        }

        internal Color32 this[int x, int y]
        {
            get { return pixels[y, x]; }
            set { pixels[y, x] = value; }
        }

        public Color GetColor(int x, int y)
        {
            return pixels[y, x].ToColor();
        }

        public void SetColor(int x, int y, Color color)
        {
            pixels[y, x] = Color32.FromColor(color);
        }

        public void WithPinnedPixels(Action<IntPtr> action)
        {
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                action(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }
    }
}