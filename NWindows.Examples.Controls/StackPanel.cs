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
                    Rectangle controlArea = new Rectangle(contentWidth, 0, controlContentSize.Width, Area.Height);
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
                    Rectangle controlArea = new Rectangle(0, contentHeight, Area.Width, controlContentSize.Height);
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
                newFreeArea = new Rectangle(contentWidth, 0, Math.Max(0, Area.Width - contentWidth), Area.Height);
            }
            else
            {
                newFreeArea = new Rectangle(0, contentHeight, Area.Width, Math.Max(0, Area.Height - contentHeight));
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
            foreach (Control control in controls)
            {
                if (control.Area.IntersectsWith(area))
                {
                    Rectangle updatedControlArea = Rectangle.Intersect(area, control.Area);
                    canvas.SetClipRectangle(updatedControlArea.X, updatedControlArea.Y, updatedControlArea.Width, updatedControlArea.Height);

                    Rectangle controlArea = new Rectangle(
                        updatedControlArea.X - control.Area.X,
                        updatedControlArea.Y - control.Area.Y,
                        updatedControlArea.Width,
                        updatedControlArea.Height
                    );
                    control.Paint(new OffsetCanvas(canvas, control.Area.X, control.Area.Y), controlArea);
                }
            }
        }

        public override void OnResize()
        {
            UpdateLayout();
        }

        public void Invalidate(Rectangle area)
        {
            Host?.Invalidate(new Rectangle(Area.Location + (Size) area.Location, area.Size));
        }
    }

    public enum StackPanelOrientation
    {
        Horizontal,
        Vertical
    }
}