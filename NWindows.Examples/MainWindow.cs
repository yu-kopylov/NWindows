using System.Drawing;

namespace NWindows.Examples
{
    public class MainWindow : BasicWindow
    {
        public MainWindow()
        {
            Title = "Examples \u2690-\xD83C\xDFC1-\u2690";
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
            canvas.FillRectangle(Color.Lime, 1, 1, 198, 100);
            canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);

            FontConfig arial = new FontConfig("Arial", 14);
            FontConfig times = new FontConfig("Times", 16).Bold().Italic().Underline().Strikeout();

            canvas.DrawString(Color.DarkBlue, arial, 180, 20, "Sample Text \u2690");
            canvas.DrawString(Color.FromArgb(0x80, Color.DarkBlue), arial, 180, 50, "Sample Text \u2690");
            canvas.DrawString(Color.DarkBlue, times, 180, 80, "Sample Text \u2690");
        }
    }
}