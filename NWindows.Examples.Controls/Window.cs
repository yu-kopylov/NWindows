using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class Window : NWindow
    {
        public Control Content
        {
            get { return content; }
            set
            {
                content?.SetTopLevelControlWindow(null);

                content = value;

                if (content != null)
                {
                    content.SetTopLevelControlWindow(this);
                    content.Area = new Rectangle(Point.Empty, ClientArea);
                }
            }
        }

        private Control content;

        internal void OnControlAdded(Control control)
        {
            if (FocusedControl == null && control.TabStop)
            {
                Focus(control);
            }
        }

        internal void OnControlRemoved(Control control)
        {
            if (FocusedControl == control)
            {
                FocusedControl = null;
            }
        }

        public Control FocusedControl
        {
            get { return focusedControl; }
            private set
            {
                if (focusedControl != value)
                {
                    if (focusedControl != null)
                    {
                        focusedControl.IsFocused = false;
                    }

                    focusedControl = value;

                    if (focusedControl != null)
                    {
                        focusedControl.IsFocused = true;
                    }
                }
            }
        }

        private Control focusedControl;

        public void MoveFocus(bool moveForward)
        {
            if (Content == null)
            {
                return;
            }

            if (FocusedControl == null && Content.TabStop)
            {
                FocusedControl = Content;
            }

            Control originalFocus = FocusedControl == null ? Content : FocusedControl;
            var getNextControl = moveForward ? (Func<Control, Control>) ControlTreeWalker.GetNextControl : ControlTreeWalker.GetPreviousControl;

            for (Control c = getNextControl(originalFocus); c != originalFocus; c = getNextControl(c))
            {
                if (c.TabStop)
                {
                    Focus(c);
                    return;
                }
            }
        }

        public void Focus(Control control)
        {
            if (control.Window != this)
            {
                throw new InvalidOperationException("Cannot set focus to a control that does not belong to this window.");
            }

            FocusedControl = control;
        }

        protected override void OnAppInit()
        {
            content?.UpdateTopLevelControlApplication();
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.White, area.X, area.Y, area.Width, area.Height);
            content?.Paint(canvas, area);
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            content?.MouseButtonDown(button, point, modifierKey);
        }

        protected override void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            if (keyCode == NKeyCode.Tab && modifierKey == NModifierKey.None)
            {
                MoveFocus(true);
                return;
            }

            if (keyCode == NKeyCode.Tab && modifierKey == NModifierKey.Shift)
            {
                MoveFocus(false);
                return;
            }

            focusedControl?.OnKeyDown(keyCode, modifierKey, autoRepeat);
        }

        protected override void OnTextInput(string text)
        {
            focusedControl?.TextInput(text);
        }

        protected override void OnResize(Size clientArea)
        {
            if (content != null)
            {
                content.Area = new Rectangle(Point.Empty, ClientArea);
            }
        }
    }
}