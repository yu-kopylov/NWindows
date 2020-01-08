using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class Window : NWindow
    {
        private Control content;

        public Control Content
        {
            get { return content; }
            set
            {
                content = value;

                if (content != null)
                {
                    content.SetTopLevelControlWindow(this);
                    content.Area = new Rectangle(Point.Empty, ClientArea);
                }
            }
        }

        protected override void OnAppInit()
        {
            content?.UpdateTopLevelControlApplication();
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            content?.Paint(canvas, area);
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            content?.OnMouseButtonDown(button, point, modifierKey);
        }

        protected override void OnResize(Size clientArea)
        {
            if (content != null)
            {
                content.Area = new Rectangle(Point.Empty, ClientArea);
            }
        }
    }
}