using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class Button : Control
    {
        public Button(string text, EventHandler clickHandler)
        {
            RepaintMode = ControlRepaintMode.Always;
            Text = text;
            PreferredSize = new Size(80, 25);
            Click += clickHandler;
        }

        public FontConfig Font { get; } = new FontConfig("Arial", 14);

        public string Text { get; }

        private bool pressed;

        public bool Pressed
        {
            get { return pressed; }
            private set
            {
                if (pressed != value)
                {
                    pressed = value;
                    InvalidatePainting();
                }
            }
        }

        public event EventHandler Click;

        public override bool TabStop => true;

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            Color borderColor = Pressed ? Color.DarkBlue : IsFocused ? Color.RoyalBlue : Color.LightGray;
            Color bgColor = Pressed ? Color.LightSkyBlue : Color.LightSteelBlue;

            canvas.FillRectangle(borderColor, 0, 0, Area.Width, Area.Height);
            canvas.FillRectangle(bgColor, 1, 1, Area.Width - 2, Area.Height - 2);
            var textSize = Application.Graphics.MeasureString(Font, Text);

            // todo: use ClippingCanvas instead of OffsetCanvas
            Rectangle textArea = Rectangle.Intersect(area, new Rectangle(2, 2, Area.Width - 4, Area.Height - 4));
            canvas.SetClipRectangle(textArea.X, textArea.Y, textArea.Width, textArea.Height);
            canvas.DrawString(Color.Black, Font, (Area.Width - textSize.Width) / 2, (Area.Height - textSize.Height) / 2, Text);
        }

        protected override void OnIsFocusedChanged()
        {
            if (!IsFocused)
            {
                ReleaseButton(false);
            }
        }

        protected override void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            if (keyCode == NKeyCode.Space && modifierKey == NModifierKey.None && !autoRepeat)
            {
                Pressed = true;
            }
        }

        protected override void OnKeyUp(NKeyCode keyCode)
        {
            ReleaseButton(keyCode == NKeyCode.Space);
        }

        protected override void OnMouseMove(Point point)
        {
            if (HasMouseCaptured)
            {
                Pressed = Area.Contains(point);
            }
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            if (button == NMouseButton.Left)
            {
                Focus();
                CaptureMouse();
                Pressed = true;
            }
        }

        protected override void OnMouseButtonUp(NMouseButton button, Point point)
        {
            if (button == NMouseButton.Left)
            {
                ReleaseButton(true);
            }
        }

        private void ReleaseButton(bool triggerClick)
        {
            ReleaseMouse();

            if (Pressed)
            {
                Pressed = false;

                if (triggerClick)
                {
                    Click?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}