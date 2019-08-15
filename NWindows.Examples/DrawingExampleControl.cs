using System.Drawing;

namespace NWindows.Examples
{
    public class DrawingExampleControl : Control
    {
        public override void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
            canvas.FillRectangle(Color.Lime, 1, 1, 198, 99);
            canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);
        }
    }
}