using System;
using NWindows.NativeApi;
using NWindows.Win32;
using NWindows.X11;

namespace NWindows
{
    public class NApplication : IDisposable
    {
        private INativeApplication NativeApp { get; }

        public NGraphics Graphics { get; }
        public NImageCodec ImageCodec { get; }
        public NClipboard Clipboard { get; }

        private NApplication(INativeApplication nativeApp)
        {
            NativeApp = nativeApp;
            Graphics = new NGraphics(nativeApp.Graphics);
            ImageCodec = new NImageCodec(nativeApp.ImageCodec);
            Clipboard = new NClipboard(nativeApp.Clipboard);
        }

        public void Dispose()
        {
            NativeApp?.Dispose();
        }

        public static NApplication Create()
        {
            INativeApplication nativeApp = null;

            try
            {
                if (X11Application.IsAvailable())
                {
                    nativeApp = X11Application.Create();
                }
                else if (Win32Application.IsAvailable())
                {
                    nativeApp = Win32Application.Create();
                }
                else
                {
                    throw new InvalidOperationException("Cannot determine a suitable API.");
                }

                var app = new NApplication(nativeApp);
                nativeApp = null;
                return app;
            }
            finally
            {
                nativeApp?.Dispose();
            }
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