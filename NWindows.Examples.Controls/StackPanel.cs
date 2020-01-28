using System;
using System.Drawing;
using System.Linq;

namespace NWindows.Examples.Controls
{
    public class StackPanel : Control
    {
        private StackPanelOrientation orientation = StackPanelOrientation.Horizontal;

        public StackPanel()
        {
            RepaintMode = ControlRepaintMode.Never;
        }

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
    }

    public enum StackPanelOrientation
    {
        Horizontal,
        Vertical
    }
}