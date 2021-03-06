using System.Drawing;

namespace NWindows
{
    public interface ICanvas
    {
        // todo: should canvas be disposable?

        void SetClipRectangle(int x, int y, int width, int height);
        void FillRectangle(Color color, int x, int y, int width, int height);
        void DrawString(Color color, FontConfig font, int x, int y, string text);
        void DrawImage(NImage image, int x, int y);
        void DrawPath(Color color, int width, Point[] points);
        void FillEllipse(Color color, int x, int y, int width, int height);
    }
}