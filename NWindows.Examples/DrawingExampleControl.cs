using System.Drawing;
using NWindows.Examples.Controls;
using NWindows.NativeApi;

namespace NWindows.Examples
{
    public class DrawingExampleControl : Control
    {
        private NImage arrow;

        public override void OnAppInit(NApplication application)
        {
            var type = typeof(Program);
            using (var stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.Resources.Images.arrow.png"))
            {
                arrow = application.ImageCodec.LoadImageFromStream(stream);
            }
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