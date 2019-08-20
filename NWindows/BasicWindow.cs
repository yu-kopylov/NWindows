using System;
using System.Drawing;

namespace NWindows
{
    public abstract class BasicWindow
    {
        private INativeApplication application;

        internal INativeApplication Application
        {
            get
            {
                if (application == null)
                {
                    throw new InvalidOperationException("This window is not associated with an application yet.");
                }

                return application;
            }
            set
            {
                if (value != application)
                {
                    application = value;
                    OnAppInit();
                }
            }
        }

        internal INativeWindow NativeWindow { get; set; }

        public IImageCodec ImageCodec => Application.ImageCodec;

        private string title;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                NativeWindow?.SetTitle(value);
            }
        }

        // todo: setter should update existing window
        public int Width { get; set; } = 800;

        // todo: setter should update existing window
        public int Height { get; set; } = 600;

        public Size ClientArea { get; internal set; }

        public void Invalidate(Rectangle area)
        {
            NativeWindow.Invalidate(area);
        }

        public abstract void Paint(ICanvas canvas, Rectangle area);

        public virtual void OnAppInit()
        {
        }

        public virtual void OnMouseMove(Point point)
        {
        }

        public virtual void OnResize(Size clientArea)
        {
        }
    }
}