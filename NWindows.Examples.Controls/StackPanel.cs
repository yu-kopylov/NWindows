using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NWindows.Examples.Controls
{
    public class StackPanel : Control, IControlHost
    {
        private readonly List<Control> controls = new List<Control>();
        private StackPanelOrientation orientation = StackPanelOrientation.Horizontal;
        private Rectangle freeArea;

        public StackPanelOrientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                UpdateLayout();
            }
        }

        public void Add(Control control)
        {
            if (control == this)
            {
                // todo: also check for loops
                throw new InvalidOperationException("Cannot add control to itself.");
            }

            controls.Add(control);
            // todo: either Host should be public or there should be a ControlCollection to allow implementation of container-controls in dependent libraries
            control.Host = this;

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            int contentHeight = 0;
            int contentWidth = 0;

            if (controls.Count != 0)
            {
                if (orientation == StackPanelOrientation.Horizontal)
                {
                    contentHeight = controls.Max(c => c.ContentSize.Height);
                }
                else
                {
                    contentWidth = controls.Max(c => c.ContentSize.Height);
                }
            }

            foreach (Control control in controls)
            {
                Size controlContentSize = control.ContentSize;

                if (orientation == StackPanelOrientation.Horizontal)
                {
                    Rectangle controlArea = new Rectangle(Area.X + contentWidth, Area.Y, controlContentSize.Width, Area.Height);
                    if (control.Area != controlArea)
                    {
                        control.Area = controlArea;
                        Invalidate(control.Area);
                    }

                    contentWidth += controlContentSize.Width;
                    contentHeight = Math.Max(contentHeight, controlContentSize.Height);
                }
                else
                {
                    Rectangle controlArea = new Rectangle(Area.X, Area.Y + contentHeight, Area.Width, controlContentSize.Height);
                    if (control.Area != controlArea)
                    {
                        control.Area = controlArea;
                        Invalidate(control.Area);
                    }

                    contentHeight += controlContentSize.Height;
                    contentWidth = Math.Max(contentWidth, controlContentSize.Width);
                }
            }

            Rectangle newFreeArea;
            if (orientation == StackPanelOrientation.Horizontal)
            {
                newFreeArea = new Rectangle(contentWidth, Area.Y, Math.Max(0, Area.Width - contentWidth), Area.Height);
            }
            else
            {
                newFreeArea = new Rectangle(Area.X, contentHeight, Area.Width, Math.Max(0, Area.Height - contentHeight));
            }

            if (freeArea != newFreeArea)
            {
                freeArea = newFreeArea;
                Invalidate(freeArea);
            }

            // todo: should ContentSize and Layout calculations be separate?
            ContentSize = new Size(contentWidth, contentHeight);
        }

        public override void OnAppInit()
        {
            // todo: call base.OnAppInit ?
            foreach (var control in controls)
            {
                control.OnAppInit();
            }
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            base.Paint(canvas, area);

            foreach (Control control in controls)
            {
                if (control.Area.IntersectsWith(area))
                {
                    control.Paint(canvas, area);
                }
            }
        }

        public override void OnPaint(ICanvas canvas, Rectangle controlArea)
        {
            // Nothing to paint. Controls are painted separately. Free area does not have its own background.
        }

        public override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            // todo: make sure that layout is updated
            foreach (var control in controls)
            {
                if (control.Area.Contains(point))
                {
                    control.OnMouseButtonDown(button, point, modifierKey);
                }
            }
        }

        public override void OnResize()
        {
            UpdateLayout();
        }
    }

    public enum StackPanelOrientation
    {
        Horizontal,
        Vertical
    }
}