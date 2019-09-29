using System.Collections.Generic;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class Window : NWindow, IControlHost
    {
        private readonly List<Control> controls = new List<Control>();

        protected override void OnAppInit()
        {
            foreach (var control in controls)
            {
                control.OnAppInit();
            }
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            foreach (Control control in controls)
            {
                // todo: test edge cases
                if (control.Area.IntersectsWith(area))
                {
                    canvas.SetClipRectangle(control.Area.X, control.Area.Y, control.Area.Width, control.Area.Height);
                    Rectangle controlArea = new Rectangle(area.X - control.Area.X, area.Y - control.Area.Y, area.Width, area.Height);
                    control.Paint(new OffsetCanvas(canvas, control.Area.X, control.Area.Y), controlArea);
                }
            }
        }

        protected void Add(Control control)
        {
            controls.Add(control);
            control.Host = this;
        }
    }
}