using System.Drawing;

namespace NWindows.NativeApi
{
    internal interface INativeCanvas
    {
        // todo: should canvas be disposable?

        void SetClipRectangle(int x, int y, int width, int height);
        void FillRectangle(Color color, int x, int y, int width, int height);
        void DrawString(Color color, FontConfig font, int x, int y, string text);
        void DrawImage(INativeImage image, int x, int y);
        void DrawPath(Color color, int width, Point[] points);
        void FillEllipse(Color color, int x, int y, int width, int height);
    }
}