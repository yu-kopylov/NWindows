using System;
using System.Drawing;
using NWindows.NativeApi;

namespace NWindows
{
    public abstract class NWindow
    {
        protected NWindow()
        {
            StartupInfo = new NWindowStartupInfo(this);
        }

        internal INativeWindowStartupInfo StartupInfo { get; }

        internal INativeWindow NativeWindow { get; set; }

        private NApplication application;

        public NApplication Application
        {
            get
            {
                if (application == null)
                {
                    throw new InvalidOperationException("This window is not associated with an application yet.");
                }

                return application;
            }
            internal set
            {
                if (value != application)
                {
                    application = value;
                    OnAppInit();
                }
            }
        }

        public NImageCodec ImageCodec => Application.ImageCodec;

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

        protected abstract void OnPaint(ICanvas canvas, Rectangle area);

        protected virtual void OnAppInit() {}

        protected virtual void OnMouseMove(Point point) {}

        protected virtual void OnKeyDown(NKeyCode keyCode, bool autoRepeat) {}

        protected virtual void OnKeyUp(NKeyCode keyCode) {}

        protected virtual void OnTextInput(string text) {}

        protected virtual void OnResize(Size clientArea) {}

        private class NWindowStartupInfo : INativeWindowStartupInfo
        {
            private readonly NWindow window;

            public NWindowStartupInfo(NWindow window)
            {
                this.window = window;
            }

            public string Title => window.Title;
            public int Width => window.Width;
            public int Height => window.Height;

            public void OnCreate(INativeWindow nativeWindow)
            {
                window.NativeWindow = nativeWindow;
            }

            public void OnKeyDown(NKeyCode keyCode, bool autoRepeat)
            {
                window.OnKeyDown(keyCode, autoRepeat);
            }

            public void OnKeyUp(NKeyCode keyCode)
            {
                window.OnKeyUp(keyCode);
            }

            public void OnTextInput(string text)
            {
                window.OnTextInput(text);
            }

            public void OnMouseMove(Point point)
            {
                window.OnMouseMove(point);
            }

            public void OnPaint(INativeCanvas canvas, Rectangle area)
            {
                window.OnPaint(new NCanvas(canvas), area);
            }

            public void OnResize(Size clientArea)
            {
                if (window.ClientArea != clientArea)
                {
                    // todo: does not look good (2 sources of client area in window)
                    window.ClientArea = clientArea;
                    window.OnResize(clientArea);
                }
            }
        }
    }
}