using System.Drawing;
using System.IO;
using NWindows.X11;

namespace NWindows.Examples
{
    public class DrawingExampleControl : Control
    {
        private IImage arrow;

        public override void OnAppInit(MainWindow mainWindow)
        {
            base.OnAppInit(mainWindow);
            string arrowPath = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "Content", "Images", "arrow.png");
            arrow = mainWindow.ImageCodec.LoadFromFile(arrowPath);
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
            canvas.FillRectangle(Color.Lime, 1, 1, 198, 99);
            canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);
            canvas.DrawImage(arrow, 68, 60);
        }
    }
}