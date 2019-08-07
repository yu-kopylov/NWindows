using NWindows.X11;

namespace NWindows
{
    public class Application
    {
        public void Run()
        {
            var app = new X11Application();
            app.Run();
        }
    }
}