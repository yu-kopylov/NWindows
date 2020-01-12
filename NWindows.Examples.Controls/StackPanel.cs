using System;
using System.Drawing;
using System.Linq;

namespace NWindows.Examples.Controls
{
    public class StackPanel : Control
    {
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

            AddChild(control);

            UpdateLayout();
        }

        public void Remove(Control control)
        {
            if (RemoveChild(control))
            {
                UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            int contentHeight = 0;
            int contentWidth = 0;

            if (Children.Count != 0)
            {
                if (orientation == StackPanelOrientation.Horizontal)
                {
                    contentHeight = Children.Max(c => c.ContentSize.Height);
                }
                else
                {
                    contentWidth = Children.Max(c => c.ContentSize.Height);
                }
            }

            foreach (Control child in Children)
            {
                Size childContentSize = child.ContentSize;

                if (orientation == StackPanelOrientation.Horizontal)
                {
                    Rectangle childArea = new Rectangle(Area.X + contentWidth, Area.Y, childContentSize.Width, Area.Height);
                    if (child.Area != childArea)
                    {
                        child.Area = childArea;
                        Invalidate(child.Area);
                    }

                    contentWidth += childContentSize.Width;
                    contentHeight = Math.Max(contentHeight, childContentSize.Height);
                }
                else
                {
                    Rectangle childArea = new Rectangle(Area.X, Area.Y + contentHeight, Area.Width, childContentSize.Height);
                    if (child.Area != childArea)
                    {
                        child.Area = childArea;
                        Invalidate(child.Area);
                    }

                    contentHeight += childContentSize.Height;
                    contentWidth = Math.Max(contentWidth, childContentSize.Width);
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

        protected override void OnPaint(ICanvas canvas, Rectangle controlArea)
        {
            // Nothing to paint. Controls are painted separately. Free area does not have its own background.
        }

        protected override void OnAreaChanged()
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