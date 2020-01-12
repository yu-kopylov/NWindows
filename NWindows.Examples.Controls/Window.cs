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

            if (MouseFocus == control)
            {
                MouseFocus = null;
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

        internal void Focus(Control control)
        {
            if (control.Window != this)
            {
                throw new InvalidOperationException("Cannot set focus to a control that does not belong to this window.");
            }

            FocusedControl = control;
        }

        internal Control MouseFocus { get; private set; }

        internal void CaptureMouse(Control control)
        {
            if (control.Window != this)
            {
                throw new InvalidOperationException("Control that does not belong to this window cannot capture mouse.");
            }

            MouseFocus = control;
        }

        internal void ReleaseMouse(Control control)
        {
            if (control == MouseFocus)
            {
                MouseFocus = null;
            }
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

        protected override void OnMouseMove(Point point)
        {
            GetMouseEventTarget(point)?.MouseMove(point);
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            GetMouseEventTarget(point)?.MouseButtonDown(button, point, modifierKey);
        }

        protected override void OnMouseButtonUp(NMouseButton button, Point point)
        {
            GetMouseEventTarget(point)?.MouseButtonUp(button, point);
        }

        private Control GetMouseEventTarget(Point point)
        {
            if (MouseFocus != null)
            {
                return MouseFocus;
            }

            if (Content == null)
            {
                return null;
            }

            Control target = Content;
            Control child = target.GetChildAtPoint(point);
            while (child != null)
            {
                target = child;
                child = target.GetChildAtPoint(point);
            }

            return target;
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

            focusedControl?.KeyDown(keyCode, modifierKey, autoRepeat);
        }

        protected override void OnKeyUp(NKeyCode keyCode)
        {
            focusedControl?.KeyUp(keyCode);
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