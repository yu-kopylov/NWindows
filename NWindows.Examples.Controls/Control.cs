using System.Drawing;

namespace NWindows.Examples.Controls
{
    public abstract class Control
    {
        private IControlHost host;
        private Rectangle area;

        public virtual IControlHost Host
        {
            get { return host; }
            internal set
            {
                host = value;
                if (host?.Application != null)
                {
                    OnAppInit();
                }
            }
        }

        public NApplication Application
        {
            get { return host?.Application; }
        }

        /// <summary>
        /// Area of the window occupied by the control.
        /// Can be outside of the window client area in case of scrolling.
        /// </summary>
        public Rectangle Area
        {
            get { return area; }
            set
            {
                if (area != value)
                {
                    area = value;
                    OnResize();
                }
            }
        }

        /// <summary>
        /// Minimum size of the control that allows fitting all its content without clipping or scaling.
        /// </summary>
        public Size ContentSize { get; set; }

        protected void Invalidate()
        {
            Invalidate(Area);
        }

        // todo: make protected
        public void Invalidate(Rectangle area)
        {
            Host?.Invalidate(area);
        }

        protected Point ToControlPoint(Point windowPoint)
        {
            return new Point(windowPoint.X - Area.X, windowPoint.Y - Area.Y);
        }

        public virtual void Paint(ICanvas canvas, Rectangle windowArea)
        {
            var controlArea = Rectangle.Intersect(windowArea, Area);
            if (controlArea.IsEmpty)
            {
                return;
            }

            canvas.SetClipRectangle(controlArea.X, controlArea.Y, controlArea.Width, controlArea.Height);
            controlArea.Offset(-Area.X, -Area.Y);
            OnPaint(new OffsetCanvas(canvas, Area.X, Area.Y), controlArea);
        }

        public abstract void OnPaint(ICanvas canvas, Rectangle area);

        public virtual void OnAppInit() {}

        public virtual void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey) {}

        public virtual void OnResize() {}
    }
}