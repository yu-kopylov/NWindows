using System;
using NWindows.NativeApi;
using NWindows.Win32;
using NWindows.X11;

namespace NWindows
{
    public class NApplication : IDisposable
    {
        private INativeApplication NativeApp { get; set; }

        public NGraphics Graphics { get; private set; }
        public NImageCodec ImageCodec { get; private set; }
        public NClipboard Clipboard { get; private set; }

        public void Dispose()
        {
            NativeApp?.Dispose();
        }

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

            Graphics = new NGraphics(NativeApp.Graphics);
            ImageCodec = new NImageCodec(NativeApp.ImageCodec);
            Clipboard = new NClipboard(NativeApp.Clipboard);
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
    }
}