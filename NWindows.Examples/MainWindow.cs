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

            StackPanel hPanel1 = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            StackPanel hPanel2 = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            StackPanel vPanel = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            vPanel.Add(hPanel1);
            vPanel.Add(hPanel2);

            Content = vPanel;

            var drawingExampleControl = new DrawingExampleControl {ContentSize = new Size(200, 250)};
            textExampleControl = new TextExampleControl {ContentSize = new Size(600, 250)};
            mouseExampleControl = new MouseExampleControl {ContentSize = new Size(200, 25)};
            keyboardExampleControl = new KeyboardExampleControl {ContentSize = new Size(200, 200)};
            textBox = new TextBox {ContentSize = new Size(200, 20)};

            hPanel1.Add(drawingExampleControl);
            hPanel1.Add(textExampleControl);
            hPanel2.Add(mouseExampleControl);
            hPanel2.Add(keyboardExampleControl);
            hPanel2.Add(textBox);
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
    }
}