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

        private Size? preferredSize;
        private Size contentSize;

        private bool childrenRequireUpdate;
        private bool requiresContentSizeUpdate;
        private bool requiresLayoutUpdate;
        private bool requiresPaintingUpdate;

        private Rectangle area;
        private Rectangle paintedArea;
        private ControlRepaintMode repaintMode = ControlRepaintMode.IncrementalGrowth;

        private ControlVisibility visibility = ControlVisibility.Visible;
        private ControlVisibility effectiveVisibility = ControlVisibility.Visible;

        private bool isFocused;

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
                RequestChildrenUpdate();
                RequestContentSizeUpdate();
                RequestLayoutUpdate();
            }
        }

        protected void RemoveChild(Control control)
        {
            if (children.Remove(control))
            {
                control.InvalidatePainting();
                control.Parent = null;
                RequestContentSizeUpdate();
                RequestLayoutUpdate();
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
                    EffectiveVisibility = CalculateEffectiveVisibility();
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
                    PaintedArea = Rectangle.Empty;
                    RequestPaintingUpdate();

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
                if (value.HasZeroArea())
                {
                    value = Rectangle.Empty;
                }

                if (area != value)
                {
                    area = value;
                    RequestLayoutUpdate();
                    RequestPaintingUpdate();
                    OnAreaChanged();
                }
            }
        }

        private Rectangle PaintedArea
        {
            get { return paintedArea; }
            set { paintedArea = value; }
        }

        public ControlRepaintMode RepaintMode
        {
            get { return repaintMode; }
            protected set
            {
                if (repaintMode != value)
                {
                    InvalidatePainting();
                    repaintMode = value;
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

        public ControlVisibility Visibility
        {
            get { return visibility; }
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    EffectiveVisibility = CalculateEffectiveVisibility();
                }
            }
        }

        public ControlVisibility EffectiveVisibility
        {
            get { return effectiveVisibility; }
            private set
            {
                if (effectiveVisibility != value)
                {
                    bool wasCollapsed = effectiveVisibility == ControlVisibility.Collapsed;
                    bool wasVisible = effectiveVisibility == ControlVisibility.Visible;

                    effectiveVisibility = value;

                    bool isCollapsed = effectiveVisibility == ControlVisibility.Collapsed;
                    bool isVisible = effectiveVisibility == ControlVisibility.Visible;

                    if (isCollapsed != wasCollapsed)
                    {
                        RequestContentSizeUpdate();
                    }

                    if (isVisible != wasVisible)
                    {
                        InvalidatePainting();
                    }

                    foreach (var child in children)
                    {
                        child.EffectiveVisibility = child.CalculateEffectiveVisibility();
                    }
                }
            }
        }

        private ControlVisibility CalculateEffectiveVisibility()
        {
            if (Parent == null)
            {
                return Visibility;
            }

            return Visibility.Combine(Parent.EffectiveVisibility);
        }

        /// <summary>
        /// Overrides <see cref="ContentSize"/> calculated by <see cref="CalculateContentSize"/>.
        /// </summary>
        public Size? PreferredSize
        {
            get { return preferredSize; }
            set
            {
                if (preferredSize != value)
                {
                    preferredSize = value;
                    if (EffectiveVisibility != ControlVisibility.Collapsed)
                    {
                        RequestContentSizeUpdate();
                    }
                }
            }
        }

        /// <summary>
        /// Minimum size of the control that allows fitting all its content without clipping or scaling.
        /// </summary>
        public Size ContentSize
        {
            get { return contentSize; }
            private set
            {
                if (contentSize != value)
                {
                    contentSize = value;
                    Parent?.InvalidateContentSize();
                    Parent?.InvalidateLayout();
                }
            }
        }

        private void RequestChildrenUpdate()
        {
            if (!childrenRequireUpdate)
            {
                childrenRequireUpdate = true;
                Parent?.RequestChildrenUpdate();
            }
        }

        protected void InvalidateContentSize()
        {
            if (PreferredSize == null && EffectiveVisibility != ControlVisibility.Collapsed)
            {
                RequestContentSizeUpdate();
            }
        }

        private void RequestContentSizeUpdate()
        {
            if (!requiresContentSizeUpdate)
            {
                requiresContentSizeUpdate = true;
                Parent?.RequestChildrenUpdate();
            }
        }

        protected void InvalidateLayout()
        {
            RequestLayoutUpdate();
        }

        private void RequestLayoutUpdate()
        {
            if (!requiresLayoutUpdate)
            {
                requiresLayoutUpdate = true;
                Parent?.RequestChildrenUpdate();
            }
        }

        protected void InvalidatePainting()
        {
            Window?.Invalidate(PaintedArea);
        }

        private void RequestPaintingUpdate()
        {
            if (!requiresPaintingUpdate)
            {
                requiresPaintingUpdate = true;
                Parent?.RequestChildrenUpdate();
            }
        }

        internal void Update()
        {
            UpdateContentSize();
            UpdateLayout();
            UpdatePainting();
            ClearChildrenUpdateRequests();
        }

        private void UpdateContentSize()
        {
            if (childrenRequireUpdate)
            {
                foreach (var child in children)
                {
                    child.UpdateContentSize();
                }
            }

            if (requiresContentSizeUpdate)
            {
                requiresContentSizeUpdate = false;
                ContentSize = EffectiveVisibility == ControlVisibility.Collapsed ? Size.Empty : (preferredSize ?? CalculateContentSize());
            }
        }

        private void UpdateLayout()
        {
            if (requiresLayoutUpdate)
            {
                requiresLayoutUpdate = false;
                PerformLayout();
            }

            if (childrenRequireUpdate)
            {
                foreach (var child in children)
                {
                    child.UpdateLayout();
                }
            }
        }

        private void UpdatePainting()
        {
            if (childrenRequireUpdate)
            {
                foreach (var child in children)
                {
                    child.UpdatePainting();
                }
            }

            if (requiresPaintingUpdate)
            {
                requiresPaintingUpdate = false;

                if (RepaintMode == ControlRepaintMode.Never)
                {
                    // nothing to do
                }
                else if (RepaintMode == ControlRepaintMode.IncrementalGrowth && PaintedArea.Location == Area.Location)
                {
                    if (Area.Width > PaintedArea.Width)
                    {
                        Window?.Invalidate(new Rectangle(Area.X + PaintedArea.Width, Area.Y, Area.Width - PaintedArea.Width, Area.Height));
                    }

                    if (Area.Height > PaintedArea.Height)
                    {
                        Window?.Invalidate(new Rectangle(Area.X, Area.Y + PaintedArea.Height, Area.Width, Area.Height - PaintedArea.Height));
                    }
                }
                else
                {
                    Window?.Invalidate(PaintedArea);
                    Window?.Invalidate(Area);
                }

                PaintedArea = Area;
            }
        }

        private void ClearChildrenUpdateRequests()
        {
            if (childrenRequireUpdate)
            {
                childrenRequireUpdate = false;
                foreach (var child in children)
                {
                    child.ClearChildrenUpdateRequests();
                }
            }

            if (requiresContentSizeUpdate || requiresLayoutUpdate || requiresPaintingUpdate)
            {
                Parent?.RequestChildrenUpdate();
            }
        }

        protected virtual Size CalculateContentSize()
        {
            return Size.Empty;
        }

        protected virtual void PerformLayout() {}

        public Control GetChildAtPoint(Point point)
        {
            Control res = null;

            foreach (var child in children)
            {
                if (child.EffectiveVisibility == ControlVisibility.Visible && child.Area.Contains(point))
                {
                    return child;
                }
            }

            return res;
        }

        protected Point ToControlPoint(Point windowPoint)
        {
            return new Point(windowPoint.X - Area.X, windowPoint.Y - Area.Y);
        }

        protected internal void Paint(ICanvas canvas, Rectangle area)
        {
            if (EffectiveVisibility != ControlVisibility.Visible)
            {
                return;
            }

            var controlArea = Rectangle.Intersect(area, Area);
            if (controlArea.HasZeroArea())
            {
                return;
            }

            canvas.SetClipRectangle(controlArea.X, controlArea.Y, controlArea.Width, controlArea.Height);
            controlArea.Offset(-Area.X, -Area.Y);
            OnPaint(new OffsetCanvas(canvas, Area.X, Area.Y), controlArea);

            PaintChildren(canvas, area);
        }

        protected void PaintChildren(ICanvas canvas, Rectangle area)
        {
            foreach (var child in children)
            {
                child.Paint(canvas, area);
            }
        }

        protected virtual void OnPaint(ICanvas canvas, Rectangle area) {}

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