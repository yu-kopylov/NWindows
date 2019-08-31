using System;
using System.Collections.Generic;
using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Examples
{
    public class MainWindow : NWindow
    {
        private readonly List<Control> controls = new List<Control>();
        private readonly TextExampleControl textExampleControl;
        private readonly MouseExampleControl mouseExampleControl;

        public MainWindow()
        {
            Title = "Examples \u2690-\xD83C\xDFC1-\u2690";

            textExampleControl = new TextExampleControl {Area = new Rectangle(200, 0, 600, 250)};
            mouseExampleControl = new MouseExampleControl {Area = new Rectangle(0, 250, 200, 25)};

            controls.Add(new DrawingExampleControl {Area = new Rectangle(0, 0, 200, 250)});
            controls.Add(textExampleControl);
            controls.Add(mouseExampleControl);
        }

        public override void OnAppInit()
        {
            foreach (var control in controls)
            {
                control.OnAppInit(this);
            }
        }

        public override void OnPaint(ICanvas canvas, Rectangle area)
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

        public override void OnResize(Size clientArea)
        {
            var newTextExampleControlArea = new Rectangle(200, 0, Math.Max(0, clientArea.Width - 200), 250);
            if (newTextExampleControlArea != textExampleControl.Area)
            {
                textExampleControl.Area = newTextExampleControlArea;
                Invalidate(textExampleControl.Area);
            }
        }
    }
}