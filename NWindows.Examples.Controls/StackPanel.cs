using System;
using System.Drawing;
using System.Linq;

namespace NWindows.Examples.Controls
{
    public class StackPanel : Control
    {
        private StackPanelOrientation orientation = StackPanelOrientation.Horizontal;

        public StackPanelOrientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    InvalidateContentSize();
                    InvalidateLayout();
                }
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
        }

        public void Remove(Control control)
        {
            RemoveChild(control);
        }

        protected override Size CalculateContentSize()
        {
            if (Children.Count == 0)
            {
                return Size.Empty;
            }

            if (orientation == StackPanelOrientation.Horizontal)
            {
                return new Size(Children.Sum(c => c.ContentSize.Width), Children.Max(c => c.ContentSize.Height));
            }

            return new Size(Children.Max(c => c.ContentSize.Width), Children.Sum(c => c.ContentSize.Height));
        }

        protected override void PerformLayout()
        {
            int offset = 0;

            foreach (Control child in Children)
            {
                Size childContentSize = child.ContentSize;

                if (orientation == StackPanelOrientation.Horizontal)
                {
                    Rectangle childArea = new Rectangle(Area.X + offset, Area.Y, childContentSize.Width, Area.Height);
                    child.Area = childArea;
                    offset += childContentSize.Width;
                }
                else
                {
                    Rectangle childArea = new Rectangle(Area.X, Area.Y + offset, Area.Width, childContentSize.Height);
                    child.Area = childArea;
                    offset += childContentSize.Height;
                }
            }
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            // Nothing to paint. Controls are painted separately. Free area does not have its own background.
        }
    }

    public enum StackPanelOrientation
    {
        Horizontal,
        Vertical
    }
}