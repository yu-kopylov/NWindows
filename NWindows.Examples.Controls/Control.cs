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
            host?.Invalidate(Area);
        }

        public abstract void Paint(ICanvas canvas, Rectangle area);

        public virtual void OnAppInit() {}

        public virtual void OnResize() {}
    }
}