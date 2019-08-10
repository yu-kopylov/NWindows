using System.Drawing;

namespace NWindows
{
    public abstract class BasicWindow
    {
        internal INativeWindow NativeWindow { get; set; }

        private string title;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                NativeWindow?.SetTitle(value);
            }
        }

        // todo: setter should update existing window
        public int Width { get; set; } = 600;

        // todo: setter should update existing window
        public int Height { get; set; } = 400;

        public abstract void Paint(ICanvas canvas, Rectangle area);
    }
}