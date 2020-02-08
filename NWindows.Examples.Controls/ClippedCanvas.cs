using System.Drawing;

namespace NWindows.Examples
{
    internal class ClippedCanvas : ICanvas
    {
        private readonly ICanvas canvas;
        private readonly Rectangle clipRectangle;

        public ClippedCanvas(ICanvas canvas, Rectangle clipRectangle)
        {
            this.canvas = canvas;
            this.clipRectangle = clipRectangle;
            canvas.SetClipRectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, clipRectangle.Height);
        }

        public void SetClipRectangle(int x, int y, int width, int height)
        {
            var rect = Rectangle.Intersect(clipRectangle, new Rectangle(x, y, width, height));
            canvas.SetClipRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            canvas.FillRectangle(color, x, y, width, height);
        }

        public void DrawString(Color color, FontConfig font, int x, int y, string text)
        {
            canvas.DrawString(color, font, x, y, text);
        }

        public void DrawImage(NImage image, int x, int y)
        {
            canvas.DrawImage(image, x, y);
        }
    }
}