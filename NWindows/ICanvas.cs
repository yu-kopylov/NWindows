using System.Drawing;

namespace NWindows
{
    public interface ICanvas
    {
        // todo: should canvas be disposable?

        void FillRectangle(Color color, int x, int y, int width, int height);
        void DrawString(Color color, FontConfig font, int x, int y, string text);
    }
}