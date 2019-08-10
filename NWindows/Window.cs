using System.Drawing;

namespace NWindows
{
    public class Window
    {
        internal INativeWindow NativeWindow { get; set; }

        // todo: setter should update title of existing window
        public string Title { get; set; }

        // todo: setter should update title of existing window
        public int Width { get; set; } = 600;

        // todo: setter should update title of existing window
        public int Height { get; set; } = 400;

        public virtual void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
            canvas.FillRectangle(Color.Lime, 1, 1, 198, 100);
            canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);
        }
    }
}