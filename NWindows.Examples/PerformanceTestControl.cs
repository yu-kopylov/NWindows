using System.Diagnostics;
using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class PerformanceTestControl : DockPanel
    {
        public PerformanceTestControl()
        {
            var innerControl = new InnerControl();
            Center = innerControl;
            Bottom = new Button("Reset Counter", (sender, args) => innerControl.ResetCounter());
        }

        class InnerControl : Control
        {
            private readonly Stopwatch overallTime = new Stopwatch();
            private int frameCount;
            private NImage arrow;

            protected override void OnApplicationChanged()
            {
                // todo: dispose image?
                if (arrow == null)
                {
                    var type = typeof(Program);
                    using (var stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.Resources.Images.arrow.png"))
                    {
                        arrow = Application.ImageCodec.LoadImageFromStream(stream);
                    }
                }
            }

            public InnerControl()
            {
                RepaintMode = ControlRepaintMode.Always;
                PreferredSize = new Size(600, 450);
            }

            public void ResetCounter()
            {
                InvalidatePainting();
                overallTime.Restart();
                frameCount = 0;
            }

            protected override void OnPaint(ICanvas canvas, Rectangle area)
            {
                Stopwatch sw = Stopwatch.StartNew();

                canvas.FillRectangle(Color.Blue, 0, 0, 200, 200);
                canvas.FillRectangle(Color.Lime, 1, 1, 198, 99);
                canvas.FillRectangle(Color.FromArgb(0x80, Color.Red), 20, 20, 160, 200);
                canvas.DrawImage(arrow, 68, 60);
                canvas.DrawPath(Color.FromArgb(0xFF, Color.Green), 5, new Point[] {new Point(10, 20), new Point(25, 40), new Point(40, 30)});
                canvas.DrawPath(Color.FromArgb(0x80, Color.Green), 5, new Point[] {new Point(10, 70), new Point(25, 90), new Point(40, 80)});

                canvas.DrawPath(Color.FromArgb(0xC0, Color.Green), 5, new Point[] {new Point(10, 50), new Point(25, 70), new Point(40, 60)});
                canvas.DrawPath(Color.FromArgb(0xFF, Color.Green), 4, new Point[] {new Point(10, 50), new Point(25, 70), new Point(40, 60)});

                canvas.FillRectangle(Color.DarkBlue, 10, 110, 31, 51);
                canvas.DrawPath(Color.FromArgb(0x80, Color.White), 0, new Point[] {new Point(10, 110), new Point(25, 130), new Point(40, 120)});
                canvas.DrawPath(Color.FromArgb(0xFF, Color.White), 0, new Point[] {new Point(10, 140), new Point(25, 160), new Point(40, 150)});

                canvas.FillRectangle(Color.DarkBlue, 60, 110, 31, 51);
                canvas.DrawPath(Color.FromArgb(0x80, Color.White), 1, new Point[] {new Point(60, 110), new Point(75, 130), new Point(90, 120)});
                canvas.DrawPath(Color.FromArgb(0xFF, Color.White), 1, new Point[] {new Point(60, 140), new Point(75, 160), new Point(90, 150)});

                canvas.FillRectangle(Color.DarkBlue, 110, 110, 31, 51);
                canvas.DrawPath(Color.FromArgb(0x80, Color.White), 2, new Point[] {new Point(110, 110), new Point(125, 130), new Point(140, 120)});
                canvas.DrawPath(Color.FromArgb(0xFF, Color.White), 2, new Point[] {new Point(110, 140), new Point(125, 160), new Point(140, 150)});

                canvas.FillRectangle(Color.Pink, 130, 70, 60, 40);
                canvas.FillEllipse(Color.FromArgb(0xFF, Color.Green), 130, 70, 60, 40);
                canvas.FillEllipse(Color.FromArgb(0x80, Color.Green), 130, 120, 60, 40);

                canvas.FillEllipse(Color.FromArgb(0x20, Color.Green), 129, 169, 62, 42);
                canvas.FillEllipse(Color.FromArgb(0xE0, Color.Green), 130, 170, 60, 40);
                canvas.FillEllipse(Color.FromArgb(0xFF, Color.Green), 131, 171, 58, 38);

                FontConfig arial = new FontConfig("Arial", 14);
                const string sampleText = "Sample Text [x-\u263A\u03BE\u2690\xD83C\xDFC1][\u96EA]";

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 250, 10, 450, 16);
                canvas.DrawString(Color.FromArgb(0xFF, Color.DarkBlue), arial, 251, 11, sampleText);

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 250, 30, 450, 16);
                canvas.DrawString(Color.FromArgb(0x80, Color.DarkBlue), arial, 251, 31, sampleText);

                FontConfig times = new FontConfig("Times New Roman", 36).Bold().Italic().Underline().Strikeout();

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 250, 50, 450, 45);
                canvas.DrawString(Color.FromArgb(0xFF, Color.DarkBlue), times, 251, 51, sampleText);

                canvas.FillRectangle(Color.FromArgb(0xFF, 0xFF, 0x98), 250, 100, 450, 45);
                canvas.DrawString(Color.FromArgb(0x80, Color.DarkBlue), times, 251, 101, sampleText);

                sw.Stop();

                double overallTimeSeconds = overallTime.Elapsed.TotalSeconds;
                frameCount++;

                const int labelAreaHeight = 40;
                canvas.FillRectangle(Color.Black, 0, Area.Height - labelAreaHeight, Area.Width, labelAreaHeight);
                canvas.DrawString(Color.White, new FontConfig("Arial", 16), 2, Area.Height - labelAreaHeight + 2, $"Paint: {area}, {sw.ElapsedMilliseconds} ms.");
                canvas.DrawString(Color.White, new FontConfig("Arial", 16), 2, Area.Height - labelAreaHeight + 22, $"FPS: {frameCount / overallTimeSeconds:#.0}");

                if (overallTime.IsRunning)
                {
                    // todo: use series of messages instead?
                    InvalidatePainting();
                }
            }
        }
    }
}