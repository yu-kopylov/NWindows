using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Examples.Controls
{
    public abstract class Control
    {
        public Rectangle Area { get; set; }

        public abstract void Paint(ICanvas canvas, Rectangle area);

        // todo: set parent instead of passing parameter
        public virtual void OnAppInit(NApplication mainWindow) {}
    }
}