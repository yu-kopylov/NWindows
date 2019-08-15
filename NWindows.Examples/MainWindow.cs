using System.Collections.Generic;
using System.Drawing;

namespace NWindows.Examples
{
    public class MainWindow : BasicWindow
    {
        private readonly List<Control> controls = new List<Control>();
        private readonly MouseExampleControl mouseExampleControl;

        public MainWindow()
        {
            Title = "Examples \u2690-\xD83C\xDFC1-\u2690";

            mouseExampleControl = new MouseExampleControl {Area = new Rectangle(0, 250, 200, 25)};

            controls.Add(new DrawingExampleControl {Area = new Rectangle(0, 0, 200, 250)});
            controls.Add(new TextExampleControl {Area = new Rectangle(200, 0, 400, 250)});
            controls.Add(mouseExampleControl);
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            foreach (Control control in controls)
            {
                // todo: test edge cases
                if (control.Area.IntersectsWith(area))
                {
                    Rectangle controlArea = new Rectangle(area.X - control.Area.X, area.Y - control.Area.Y, area.Width, area.Height);
                    control.Paint(new OffsetCanvas(canvas, control.Area.X, control.Area.Y), controlArea);
                }
            }
        }

        public override void OnMouseMove(Point point)
        {
            mouseExampleControl.MousePosition = point;
            Invalidate(mouseExampleControl.Area);
        }
    }
}