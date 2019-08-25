namespace NWindows.Win32
{
    public class W32Image : IImage
    {
        public W32Image(int width, int height, byte[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public int Width { get; }

        public int Height { get; }

        public byte[] Pixels { get; }
    }
}