using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class MouseExampleControl : Control
    {
        private Point mousePosition;

        public Point MousePosition
        {
            get { return mousePosition; }
            set
            {
                mousePosition = value;
                Invalidate();
            }
        }

        public override void OnPaint(ICanvas canvas, Rectangle area)
        {
            FontConfig arial = new FontConfig("Arial", 16);
            canvas.FillRectangle(Color.LightBlue, 0, 0, 200, 25);
            canvas.DrawString(Color.Black, arial, 1, 1, $"Mouse: {MousePosition}");
        }
    }
}