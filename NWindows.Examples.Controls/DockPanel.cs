using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class DockPanel : Control
    {
        private Control top;
        private Control bottom;
        private Control left;
        private Control right;
        private Control center;

        public DockPanel()
        {
            RepaintMode = ControlRepaintMode.Never;
        }

        public Control Top
        {
            get { return top; }
            set { ReplaceChild(ref top, value); }
        }

        public Control Bottom
        {
            get { return bottom; }
            set { ReplaceChild(ref bottom, value); }
        }

        public Control Left
        {
            get { return left; }
            set { ReplaceChild(ref left, value); }
        }

        public Control Right
        {
            get { return right; }
            set { ReplaceChild(ref right, value); }
        }

        public Control Center
        {
            get { return center; }
            set { ReplaceChild(ref center, value); }
        }

        private void ReplaceChild(ref Control child, Control value)
        {
            if (child == value)
            {
                return;
            }

            if (child != null)
            {
                RemoveChild(child);
            }

            if (value != null)
            {
                AddChild(value);
            }

            child = value;
        }

        protected override Size CalculateContentSize()
        {
            var topSize = Top?.ContentSize ?? Size.Empty;
            var bottomSize = Bottom?.ContentSize ?? Size.Empty;
            var leftSize = Left?.ContentSize ?? Size.Empty;
            var rightSize = Right?.ContentSize ?? Size.Empty;
            var centerSize = Center?.ContentSize ?? Size.Empty;

            var centerGroupWidth = leftSize.Width + centerSize.Width + rightSize.Width;
            var centerGroupHeight = Math.Max(Math.Max(leftSize.Height, centerSize.Height), rightSize.Height);

            int width = Math.Max(Math.Max(topSize.Width, centerGroupWidth), bottomSize.Width);
            int height = topSize.Height + centerGroupHeight + bottomSize.Height;

            return new Size(width, height);
        }

        protected override void PerformLayout()
        {
            var topSize = Top == null ? Size.Empty : new Size(Area.Width, Math.Min(Top.ContentSize.Height, Area.Height));
            var bottomSize = Bottom == null ? Size.Empty : new Size(Area.Width, Math.Min(Bottom.ContentSize.Height, Area.Height));
            int centerGroupHeight = Math.Max(0, Area.Height - topSize.Height - bottomSize.Height);
            var leftSize = Left == null ? Size.Empty : new Size(Math.Min(Left.ContentSize.Width, Area.Width), centerGroupHeight);
            var rightSize = Right == null ? Size.Empty : new Size(Math.Min(Right.ContentSize.Width, Area.Width), centerGroupHeight);
            var centerWidth = Math.Max(0, Area.Width - leftSize.Width - rightSize.Width);
            var centerSize = Center == null ? Size.Empty : new Size(centerWidth, centerGroupHeight);

            if (Top != null)
            {
                Top.Area = new Rectangle(Area.X, Area.Y, topSize.Width, topSize.Height);
            }

            if (Bottom != null)
            {
                Bottom.Area = new Rectangle(Area.X, Area.Y + Area.Height - bottomSize.Height, bottomSize.Width, bottomSize.Height);
            }

            if (Left != null)
            {
                Left.Area = new Rectangle(Area.X, Area.Y + topSize.Height, leftSize.Width, leftSize.Height);
            }

            if (Right != null)
            {
                Right.Area = new Rectangle(Area.X + Area.Width - rightSize.Width, Area.Y + topSize.Height, rightSize.Width, rightSize.Height);
            }

            if (Center != null)
            {
                Center.Area = new Rectangle(Area.X + leftSize.Width, Area.Y + topSize.Height, centerSize.Width, centerSize.Height);
            }
        }
    }
}