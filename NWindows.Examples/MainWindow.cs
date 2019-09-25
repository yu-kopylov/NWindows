using System;
using System.Collections.Generic;
using System.Drawing;
using NWindows.Examples.Controls;
using NWindows.NativeApi;

namespace NWindows.Examples
{
    public class MainWindow : NWindow
    {
        private readonly List<Control> controls = new List<Control>();
        private readonly TextExampleControl textExampleControl;
        private readonly MouseExampleControl mouseExampleControl;
        private readonly KeyboardExampleControl keyboardExampleControl;
        private readonly TextBox textBox;

        public MainWindow()
        {
            Title = "Examples \u2690-\xD83C\xDFC1-\u2690";

            var drawingExampleControl = new DrawingExampleControl {Area = new Rectangle(0, 0, 200, 250)};
            textExampleControl = new TextExampleControl {Area = new Rectangle(200, 0, 600, 250)};
            mouseExampleControl = new MouseExampleControl {Area = new Rectangle(0, 250, 200, 25)};
            keyboardExampleControl = new KeyboardExampleControl {Area = new Rectangle(200, 250, 200, 200)};
            textBox = new TextBox {Area = new Rectangle(450, 250, 200, 20)};

            controls.Add(drawingExampleControl);
            controls.Add(textExampleControl);
            controls.Add(mouseExampleControl);
            controls.Add(keyboardExampleControl);
            controls.Add(textBox);
        }

        protected override void OnAppInit()
        {
            foreach (var control in controls)
            {
                control.OnAppInit(this);
            }
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            foreach (Control control in controls)
            {
                // todo: test edge cases
                if (control.Area.IntersectsWith(area))
                {
                    canvas.SetClipRectangle(control.Area.X, control.Area.Y, control.Area.Width, control.Area.Height);
                    Rectangle controlArea = new Rectangle(area.X - control.Area.X, area.Y - control.Area.Y, area.Width, area.Height);
                    control.Paint(new OffsetCanvas(canvas, control.Area.X, control.Area.Y), controlArea);
                }
            }
        }

        protected override void OnMouseMove(Point point)
        {
            mouseExampleControl.MousePosition = point;
            Invalidate(mouseExampleControl.Area);
        }

        protected override void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            keyboardExampleControl.HandleKeyDown(keyCode, modifierKey, autoRepeat);
            textBox.OnKeyDown(keyCode, modifierKey, autoRepeat);

            Invalidate(keyboardExampleControl.Area);
            Invalidate(textBox.Area);
        }

        protected override void OnKeyUp(NKeyCode keyCode)
        {
            keyboardExampleControl.HandleKeyUp(keyCode);
            Invalidate(keyboardExampleControl.Area);
        }

        protected override void OnTextInput(string text)
        {
            keyboardExampleControl.HandleTextInput(text);
            textBox.OnTextInput(text);

            Invalidate(keyboardExampleControl.Area);
            Invalidate(textBox.Area);
        }

        protected override void OnResize(Size clientArea)
        {
            var newTextExampleControlArea = new Rectangle(200, 0, Math.Max(0, clientArea.Width - 200), 250);
            if (newTextExampleControlArea != textExampleControl.Area)
            {
                textExampleControl.Area = newTextExampleControlArea;
                Invalidate(textExampleControl.Area);
            }
        }
    }
}