using System.Drawing;

namespace NWindows
{
    /// <summary>
    /// <para>Represents a platform-independent image suitable for direct access to pixels by native and managed libraries.</para>
    /// <para>Image format is 32 bits per pixel ARGB with straight alpha.</para>
    /// </summary>
    public class NBitmap
    {
        private readonly Color32[,] pixels;

        public int Width { get; }
        public int Height { get; }

        public NBitmap(int width, int height)
        {
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
            return pixels[x, y].ToColor();
        }

        public void SetColor(int x, int y, Color color)
        {
            pixels[x, y] = Color32.FromColor(color);
        }
    }
}