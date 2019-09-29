using System;
using NWindows.NativeApi;
using NWindows.Win32;
using NWindows.X11;

namespace NWindows
{
    public class NApplication
    {
        private INativeApplication NativeApp { get; set; }

        public void Init()
        {
            if (NativeApp != null)
            {
                throw new InvalidOperationException("Application was already initialized.");
            }

            if (X11Application.IsAvailable())
            {
                NativeApp = new X11Application();
            }
            else if (Win32Application.IsAvailable())
            {
                NativeApp = new Win32Application();
            }
            else
            {
                throw new InvalidOperationException("Cannot determine a suitable API.");
            }

            NativeApp.Init();

            Graphics = new NGraphics(NativeApp.CreateGraphics());
            ImageCodec = new NImageCodec(NativeApp.CreateImageCodec());
        }

        public void Run(NWindow window)
        {
            if (NativeApp == null)
            {
                throw new InvalidOperationException("Application is not initialized yet.");
            }

            window.Application = this;
            NativeApp.Run(window.StartupInfo);
        }

        public NGraphics Graphics { get; private set; }
        public NImageCodec ImageCodec { get; private set; }
    }
}