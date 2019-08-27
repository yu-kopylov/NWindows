using System;
using NWindows.Win32;
using NWindows.X11;

namespace NWindows
{
    public class Application
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
        }

        public void Run(BasicWindow window)
        {
            if (NativeApp == null)
            {
                throw new InvalidOperationException("Application is not initialized yet.");
            }

            NativeApp.Run(window);
        }

        public IImageCodec ImageCodec => NativeApp.ImageCodec;
    }
}