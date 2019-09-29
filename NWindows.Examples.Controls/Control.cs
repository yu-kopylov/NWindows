using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public abstract class Control
    {
        private IControlHost host;

        public virtual IControlHost Host
        {
            get { return host; }
            internal set
            {
                host = value;
                if (host.Application != null)
                {
                    OnAppInit();
                }
            }
        }

        public NApplication Application
        {
            get
            {
                if (host == null)
                {
                    throw new InvalidOperationException("This control is not associated with an host yet.");
                }

                return host.Application;
            }
        }

        public Rectangle Area { get; set; }

        protected void Invalidate()
        {
            host?.Invalidate(Area);
        }

        public abstract void Paint(ICanvas canvas, Rectangle area);

        // todo: set parent instead of passing parameter
        public virtual void OnAppInit() {}
    }
}