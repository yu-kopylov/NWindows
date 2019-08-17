using System.Drawing;

namespace NWindows.Examples
{
    public class TextExampleControl : Control
    {
        public override void Paint(ICanvas canvas, Rectangle area)
        {
            const string sampleText = "Sample Text [x-\u263A\u03BE\u2690\xD83C\xDFC1][\u96EA]";

            FontConfig arial = new FontConfig("Arial", 14);

            canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 10, 10, 580, 16);
            canvas.DrawString(Color.Blue, arial, 11, 11, sampleText);

            canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 10, 30, 580, 16);
            canvas.DrawString(Color.FromArgb(0x80, Color.Blue), arial, 11, 31, sampleText);

            FontConfig times = new FontConfig("Times New Roman", 36).Bold().Italic().Underline().Strikeout();

            canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 10, 50, 580, 45);
            canvas.DrawString(Color.DarkBlue, times, 11, 51, sampleText);
        }
    }
}