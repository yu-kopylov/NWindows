using System;
using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class MainWindow : Window
    {
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

            Add(drawingExampleControl);
            Add(textExampleControl);
            Add(mouseExampleControl);
            Add(keyboardExampleControl);
            Add(textBox);
        }

        protected override void OnMouseMove(Point point)
        {
            mouseExampleControl.MousePosition = point;
        }

        protected override void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            keyboardExampleControl.HandleKeyDown(keyCode, modifierKey, autoRepeat);
            textBox.OnKeyDown(keyCode, modifierKey, autoRepeat);
        }

        protected override void OnKeyUp(NKeyCode keyCode)
        {
            keyboardExampleControl.HandleKeyUp(keyCode);
        }

        protected override void OnTextInput(string text)
        {
            keyboardExampleControl.HandleTextInput(text);
            textBox.OnTextInput(text);
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