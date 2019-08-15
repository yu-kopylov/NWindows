using System.Drawing;

namespace NWindows.Examples
{
    public class TextExampleControl : Control
    {
        public override void Paint(ICanvas canvas, Rectangle area)
        {
            FontConfig arial = new FontConfig("Arial", 14);

            canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 10, 10, 380, 16);
            canvas.DrawString(Color.Blue, arial, 11, 11, "Sample Text \u2690");

            canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 10, 30, 380, 16);
            canvas.DrawString(Color.FromArgb(0x80, Color.Blue), arial, 11, 31, "Sample Text \u2690");

            FontConfig times = new FontConfig("Times", 36).Bold().Italic().Underline().Strikeout();

            canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 10, 50, 380, 45);
            canvas.DrawString(Color.DarkBlue, times, 11, 51, "Sample Text (x) \u2690");
        }
    }
}