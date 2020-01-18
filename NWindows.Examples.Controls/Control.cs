using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public abstract class Control
    {
        private Control parent;
        private Window window;
        private NApplication application;
        private readonly LinkedHashSet<Control> children = new LinkedHashSet<Control>(ReferenceEqualityComparer<Control>.Instance);

        private Rectangle area;

        internal void SetTopLevelControlWindow(Window window)
        {
            if (Parent != null)
            {
                throw new InvalidOperationException($"{nameof(Window)} can only be set directly only on a top level control.");
            }

            Window = window;
        }

        internal void UpdateTopLevelControlApplication()
        {
            if (Parent != null)
            {
                throw new InvalidOperationException($"{nameof(Application)} can only be set directly only on a top level control.");
            }

            Application = Window?.Application;
        }

        public IReadOnlyLinkedHashSet<Control> Children => children;

        protected void AddChild(Control control)
        {
            if (children.Add(control))
            {
                control.Parent = this;
                InvalidateContentSize();
                InvalidateLayout();
            }
        }

        protected void RemoveChild(Control control)
        {
            if (children.Remove(control))
            {
                control.Parent = null;
                InvalidateContentSize();
                InvalidateLayout();
            }
        }

        public Control Parent
        {
            get { return parent; }
            private set
            {
                if (parent != value)
                {
                    parent = value;
                    Window = parent?.Window;
                }
            }
        }

        public Window Window
        {
            get { return window; }
            private set
            {
                if (window != value)
                {
                    window?.OnControlRemoved(this);

                    window = value;
                    Application = window?.Application;

                    window?.OnControlAdded(this);

                    foreach (var child in children)
                    {
                        child.Window = window;
                    }
                }
            }
        }

        public NApplication Application
        {
            get { return application; }
            private set
            {
                if (application != value)
                {
                    application = value;
                    OnApplicationChanged();
                    foreach (var child in children)
                    {
                        child.Application = application;
                    }
                }
            }
        }

        /// <summary>
        /// Area of the window occupied by the control.
        /// Can be outside of the window client area in case of scrolling.
        /// </summary>
        public Rectangle Area
        {
            get { return area; }
            set
            {
                if (area != value)
                {
                    area = value;
                    InvalidateLayout();
                    OnAreaChanged();
                }
            }
        }

        public virtual bool TabStop => false;

        public bool IsFocused
        {
            get { return isFocused; }
            internal set
            {
                if (isFocused != value)
                {
                    isFocused = value;
                    InvalidatePainting();
                    OnIsFocusedChanged();
                }
            }
        }

        private bool isFocused;

        public void Focus()
        {
            Window?.Focus(this);
        }

        public void CaptureMouse()
        {
            Window?.CaptureMouse(this);
        }

        public void ReleaseMouse()
        {
            Window?.ReleaseMouse(this);
        }

        public bool HasMouseCaptured => Window?.MouseFocus == this;

        /// <summary>
        /// Minimum size of the control that allows fitting all its content without clipping or scaling.
        /// </summary>
        public Size ContentSize
        {
            get { return contentSize; }
            set
            {
                contentSize = value;
                Parent?.InvalidateContentSize();
                Parent?.InvalidateLayout();
            }
        }

        private Size contentSize;
        private bool requiresContentSizeUpdate = true;
        private bool requiresLayoutUpdate = true;
        private bool childRequiresLayoutUpdate = true;

        protected void InvalidateContentSize()
        {
            if (!requiresContentSizeUpdate)
            {
                requiresContentSizeUpdate = true;
                Parent?.InvalidateContentSize();
            }
        }

        private void UpdateContentSize()
        {
            if (!requiresContentSizeUpdate)
            {
                return;
            }

            foreach (var child in Children)
            {
                child.UpdateContentSize();
            }

            requiresContentSizeUpdate = false;
            CalculateContentSize();
        }

        protected void InvalidateLayout()
        {
            if (!requiresLayoutUpdate)
            {
                requiresLayoutUpdate = true;
                Parent?.InvalidateChildLayout();
            }
        }

        private void InvalidateChildLayout()
        {
            if (!childRequiresLayoutUpdate)
            {
                childRequiresLayoutUpdate = true;
                Parent?.InvalidateChildLayout();
            }
        }

        internal void UpdateLayout()
        {
            UpdateContentSize();

            if (requiresLayoutUpdate)
            {
                requiresLayoutUpdate = false;
                PerformLayout();
            }

            if (childRequiresLayoutUpdate)
            {
                foreach (var child in Children)
                {
                    child.UpdateLayout();
                }

                childRequiresLayoutUpdate = false;
            }
        }

        protected virtual void CalculateContentSize() {}
        protected virtual void PerformLayout() {}

        public Control GetChildAtPoint(Point point)
        {
            foreach (var child in Children)
            {
                if (child.Area.Contains(point))
                {
                    return child;
                }
            }

            return null;
        }

        protected void InvalidatePainting()
        {
            InvalidatePainting(Area);
        }

        protected void InvalidatePainting(Rectangle area)
        {
            Window?.Invalidate(area);
        }

        protected Point ToControlPoint(Point windowPoint)
        {
            return new Point(windowPoint.X - Area.X, windowPoint.Y - Area.Y);
        }

        protected internal void Paint(ICanvas canvas, Rectangle area)
        {
            var controlArea = Rectangle.Intersect(area, Area);
            if (controlArea.Width == 0 || controlArea.Height == 0)
            {
                return;
            }

            canvas.SetClipRectangle(controlArea.X, controlArea.Y, controlArea.Width, controlArea.Height);
            controlArea.Offset(-Area.X, -Area.Y);
            OnPaint(new OffsetCanvas(canvas, Area.X, Area.Y), controlArea);

            PaintChildren(canvas, area);
        }

        protected virtual void PaintChildren(ICanvas canvas, Rectangle area)
        {
            foreach (Control child in children)
            {
                child.Paint(canvas, area);
            }
        }

        protected abstract void OnPaint(ICanvas canvas, Rectangle area);

        protected virtual void OnApplicationChanged() {}

        protected virtual void OnAreaChanged() {}

        protected virtual void OnIsFocusedChanged() {}

        internal void MouseMove(Point point)
        {
            OnMouseMove(point);
        }

        internal void MouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            OnMouseButtonDown(button, point, modifierKey);
        }

        internal void MouseButtonUp(NMouseButton button, Point point)
        {
            OnMouseButtonUp(button, point);
        }

        internal void KeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            OnKeyDown(keyCode, modifierKey, autoRepeat);
        }

        internal void KeyUp(NKeyCode keyCode)
        {
            OnKeyUp(keyCode);
        }

        internal void TextInput(string text)
        {
            OnTextInput(text);
        }

        protected virtual void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat) {}

        protected virtual void OnKeyUp(NKeyCode keyCode) {}

        protected virtual void OnMouseMove(Point point) {}

        protected virtual void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey) {}

        protected virtual void OnMouseButtonUp(NMouseButton button, Point point) {}

        protected virtual void OnTextInput(string text) {}
    }
}