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
            }
        }

        protected bool RemoveChild(Control control)
        {
            if (children.Remove(control))
            {
                control.Parent = null;
                return true;
            }

            return false;
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
                    Invalidate();
                }
            }
        }

        private bool isFocused;

        public void Focus()
        {
            Window?.Focus(this);
        }

        /// <summary>
        /// Minimum size of the control that allows fitting all its content without clipping or scaling.
        /// </summary>
        public Size ContentSize { get; set; }

        protected void Invalidate()
        {
            Window?.Invalidate(Area);
        }

        protected void Invalidate(Rectangle area)
        {
            Window?.Invalidate(area);
        }

        protected Point ToControlPoint(Point windowPoint)
        {
            return new Point(windowPoint.X - Area.X, windowPoint.Y - Area.Y);
        }

        protected internal virtual void Paint(ICanvas canvas, Rectangle area)
        {
            var controlArea = Rectangle.Intersect(area, Area);
            if (controlArea.IsEmpty)
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

        // todo: fix access modifiers

        public virtual void OnApplicationChanged() {}

        public virtual void OnAreaChanged() {}

        internal void MouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            OnMouseButtonDown(button, point, modifierKey);
        }

        public virtual void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat) {}

        internal void TextInput(string text)
        {
            OnTextInput(text);
        }

        protected virtual void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey) {}

        protected virtual void OnTextInput(string text) {}
    }
}