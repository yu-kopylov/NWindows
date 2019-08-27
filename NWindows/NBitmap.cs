using System.Drawing;

namespace NWindows
{
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

        public Color32 this[int x, int y]
        {
            get { return pixels[x, y]; }
            set { pixels[x, y] = value; }
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