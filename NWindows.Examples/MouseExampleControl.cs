using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Examples
{
    public class MouseExampleControl : Control
    {
        public Point MousePosition { get; set; }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            FontConfig arial = new FontConfig("Arial", 16);
            canvas.FillRectangle(Color.LightBlue, 0, 0, 200, 25);
            canvas.DrawString(Color.Black, arial, 1, 1, $"Mouse: {MousePosition}");
        }
    }
}