using System.Drawing;

namespace NWindows
{
    public class Window
    {
        internal INativeWindow NativeWindow { get; set; }

        // todo: setter should update title of existing window
        public string Title { get; set; }

        public virtual void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, 0, 0, 500, 350);
            canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
            canvas.FillRectangle(Color.Lime, 1, 1, 198, 100);
            canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);
        }
    }
}