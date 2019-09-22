using System.Drawing;

namespace NWindows.NativeApi
{
    public interface ICanvas
    {
        // todo: should canvas be disposable?

        void SetClipRectangle(int x, int y, int width, int height);
        void FillRectangle(Color color, int x, int y, int width, int height);
        void DrawString(Color color, FontConfig font, int x, int y, string text);
        void DrawImage(NImage image, int x, int y);
    }
}