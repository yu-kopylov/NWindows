using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class VisibilityExampleControl : StackPanel
    {
        public VisibilityExampleControl()
        {
            RepaintMode = ControlRepaintMode.IncrementalGrowth;
            Orientation = StackPanelOrientation.Vertical;
            var p1b = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            var p2 = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            var p2b = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            var tb = new TextBox {PreferredSize = new Size(200, 20), Text = "Test"};

            Add(p1b);
            Add(p2);
            p2.Add(p2b);
            p2.Add(tb);

            p1b.Add(new Button("Show", (_, __) => p2.Visibility = ControlVisibility.Visible));
            p1b.Add(new Button("Hide", (_, __) => p2.Visibility = ControlVisibility.Hidden));
            p1b.Add(new Button("Collapse", (_, __) => p2.Visibility = ControlVisibility.Collapsed));

            p2b.Add(new Button("Show", (_, __) => tb.Visibility = ControlVisibility.Visible));
            p2b.Add(new Button("Hide", (_, __) => tb.Visibility = ControlVisibility.Hidden));
            p2b.Add(new Button("Collapse", (_, __) => tb.Visibility = ControlVisibility.Collapsed));
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            base.OnPaint(canvas, area);
            canvas.FillRectangle(Color.Thistle, area.X, area.Y, area.Width, area.Height);
        }
    }
}