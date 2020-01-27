using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class MainWindow : Window
    {
        private readonly MouseExampleControl mouseExampleControl;
        private readonly EventsExampleControl eventsExampleControl;

        public MainWindow()
        {
            Title = "Examples \u2690-\xD83C\xDFC1-\u2690";

            mouseExampleControl = new MouseExampleControl {PreferredSize = new Size(200, 25)};
            eventsExampleControl = new EventsExampleControl {PreferredSize = new Size(350, 250)};

            var dockPanel = new DockPanel();
            var menuPanel = new StackPanel {Orientation = StackPanelOrientation.Vertical};

            dockPanel.Left = menuPanel;

            var mainContent = CreateMainContent();
            dockPanel.Center = mainContent;

            menuPanel.Add(new Button("Main", (_, __) => dockPanel.Center = mainContent));
            menuPanel.Add(new Button("Button", (_, __) => dockPanel.Center = new Button("Button", (___, ____) => {})));

            Content = dockPanel;
        }

        private Control CreateMainContent()
        {
            StackPanel hPanel1 = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            StackPanel hPanel2 = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            StackPanel vPanel = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            vPanel.Add(hPanel1);
            vPanel.Add(hPanel2);

            var drawingExampleControl = new DrawingExampleControl {PreferredSize = new Size(200, 250)};

            hPanel1.Add(drawingExampleControl);

            var vPanel3 = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            vPanel3.Add(new VisibilityExampleControl());
            vPanel3.Add(new TextExampleControl {PreferredSize = new Size(600, 220)});
            hPanel1.Add(vPanel3);

            hPanel2.Add(mouseExampleControl);
            hPanel2.Add(eventsExampleControl);
            var vPanel2 = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            hPanel2.Add(vPanel2);
            vPanel2.Add(new TextBox {PreferredSize = new Size(200, 20), Text = "TextBox Test 1"});
            var textBox2 = new TextBox {PreferredSize = new Size(200, 20), Text = "TextBox Test 2"};
            vPanel2.Add(textBox2);
            var hPanel3 = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            vPanel2.Add(hPanel3);
            hPanel3.Add(new Button("+A", (_, __) => textBox2.Text += "A"));
            hPanel3.Add(new Button("+B", (_, __) => textBox2.Text += "B"));

            return vPanel;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            eventsExampleControl.HandleWindowActivated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            eventsExampleControl.HandleWindowDeactivated();
        }

        protected override void OnMouseMove(Point point)
        {
            base.OnMouseMove(point);
            mouseExampleControl.MousePosition = point;
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            base.OnMouseButtonDown(button, point, modifierKey);
            eventsExampleControl.HandleMouseButtonDown(button, point, modifierKey);
        }

        protected override void OnMouseButtonUp(NMouseButton button, Point point)
        {
            base.OnMouseButtonUp(button, point);
            eventsExampleControl.HandleMouseButtonUp(button, point);
        }

        protected override void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            base.OnKeyDown(keyCode, modifierKey, autoRepeat);
            eventsExampleControl.HandleKeyDown(keyCode, modifierKey, autoRepeat);
        }

        protected override void OnKeyUp(NKeyCode keyCode)
        {
            base.OnKeyUp(keyCode);
            eventsExampleControl.HandleKeyUp(keyCode);
        }

        protected override void OnTextInput(string text)
        {
            base.OnTextInput(text);
            eventsExampleControl.HandleTextInput(text);
        }
    }
}