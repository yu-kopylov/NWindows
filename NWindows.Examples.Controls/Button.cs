using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class Button : Control
    {
        public Button(string text, EventHandler clickHandler)
        {
            Text = text;
            ContentSize = new Size(80, 25);
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
                    Invalidate();
                }
            }
        }

        public event EventHandler Click;

        public override bool TabStop => true;

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            Color borderColor = Pressed ? Color.DarkBlue : IsFocused ? Color.RoyalBlue : Color.LightGray;
            Color bgColor = Pressed ? Color.LightSkyBlue : Color.LightSteelBlue;

            canvas.FillRectangle(borderColor, 0, 0, ContentSize.Width, ContentSize.Height);
            canvas.FillRectangle(bgColor, 1, 1, ContentSize.Width - 2, ContentSize.Height - 2);
            var textSize = Application.Graphics.MeasureString(Font, Text);
            canvas.SetClipRectangle(2, 2, ContentSize.Width - 4, ContentSize.Height - 4);
            canvas.DrawString(Color.Black, Font, (ContentSize.Width - textSize.Width) / 2, (ContentSize.Height - textSize.Height) / 2, Text);
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