using System;
using System.Drawing;

namespace NWindows.Examples
{
    public class MainWindow : BasicWindow
    {
        private Point mousePosition;

        public MainWindow()
        {
            Title = "Examples \u2690-\xD83C\xDFC1-\u2690";
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);

            if (area.Top < 250)
            {
                canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
                canvas.FillRectangle(Color.Lime, 1, 1, 198, 100);
                canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);

                FontConfig arial = new FontConfig("Arial", 14);

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 210, 10, 300, 16);
                canvas.DrawString(Color.Blue, arial, 211, 11, "Sample Text \u2690");

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 210, 30, 300, 16);
                canvas.DrawString(Color.FromArgb(0x80, Color.Blue), arial, 211, 31, "Sample Text \u2690");

                FontConfig times = new FontConfig("Times", 36).Bold().Italic().Underline().Strikeout();

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 210, 50, 300, 45);
                canvas.DrawString(Color.DarkBlue, times, 211, 51, "Sample Text (x) \u2690");
            }

            if (area.Top <= 271 && area.Bottom >= 250)
            {
                FontConfig arial = new FontConfig("Arial", 16);
                canvas.FillRectangle(Color.LightBlue, 10, 250, 200, 21);
                canvas.DrawString(Color.Black, arial, 11, 251, $"Mouse: {mousePosition}");
            }
        }

        public override void OnMouseMove(Point point)
        {
            mousePosition = point;
            Invalidate(new Rectangle(10, 250, 200, 21));
        }
    }
}