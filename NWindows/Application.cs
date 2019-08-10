using System;
using NWindows.Win32;
using NWindows.X11;

namespace NWindows
{
    public class Application
    {
        public void Run(BasicWindow window)
        {
            if (X11Application.IsAvailable())
            {
                var app = new X11Application();
                app.Run(window);
            }
            else if (Win32Application.IsAvailable())
            {
                var app = new Win32Application();
                app.Run(window);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine a suitable API.");
            }
        }
    }
}