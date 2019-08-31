using System.Drawing;
using NWindows.NativeApi;

namespace NWindows
{
    public class NCanvas : ICanvas
    {
        internal NCanvas(INativeCanvas nativeCanvas)
        {
            NativeCanvas = nativeCanvas;
        }

        internal INativeCanvas NativeCanvas { get; }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            NativeCanvas.FillRectangle(color, x, y, width, height);
        }

        public void DrawString(Color color, FontConfig font, int x, int y, string text)
        {
            NativeCanvas.DrawString(color, font, x, y, text);
        }

        public void DrawImage(NImage image, int x, int y)
        {
            NativeCanvas.DrawImage(image.NativeImage, x, y);
        }
    }
}