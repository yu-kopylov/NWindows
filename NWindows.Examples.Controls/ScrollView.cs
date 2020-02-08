using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class ScrollView : Control
    {
        private readonly ScrollBar hScrollBar;
        private readonly ScrollBar vScrollBar;
        private readonly ContentContainer contentContainer;

        public ScrollView()
        {
            RepaintMode = ControlRepaintMode.Never;

            hScrollBar = new ScrollBar {Orientation = ScrollBarOrientation.Horizontal};
            vScrollBar = new ScrollBar {Orientation = ScrollBarOrientation.Vertical};
            contentContainer = new ContentContainer(hScrollBar, vScrollBar);

            AddChild(contentContainer);
            AddChild(vScrollBar);
            AddChild(hScrollBar);

            hScrollBar.MinValue = 0;
            vScrollBar.MinValue = 0;

            hScrollBar.ValueChanged += (_, __) => contentContainer.ContentLocation = new Point(-hScrollBar.Value, -vScrollBar.Value);
            vScrollBar.ValueChanged += (_, __) => contentContainer.ContentLocation = new Point(-hScrollBar.Value, -vScrollBar.Value);
        }

        public Control Content
        {
            get { return contentContainer.Content; }
            set { contentContainer.Content = value; }
        }

        protected override Size CalculateContentSize()
        {
            int width = contentContainer.ContentSize.Width + vScrollBar.ContentSize.Width;
            int height = contentContainer.ContentSize.Height + hScrollBar.ContentSize.Height;
            return new Size(width, height);
        }

        protected override void PerformLayout()
        {
            int hScrollBarHeight = Math.Min(Area.Height, hScrollBar.ContentSize.Height);
            int vScrollBarWidth = Math.Min(Area.Width, vScrollBar.ContentSize.Width);
            int contentContainerWidth = Area.Width - vScrollBarWidth;
            int contentContainerHeight = Area.Height - hScrollBarHeight;

            contentContainer.Area = new Rectangle(Area.X, Area.Y, contentContainerWidth, contentContainerHeight);
            hScrollBar.Area = new Rectangle(Area.X, Area.Y + Area.Height - hScrollBarHeight, contentContainerWidth, hScrollBar.ContentSize.Height);
            vScrollBar.Area = new Rectangle(Area.X + Area.Width - vScrollBarWidth, Area.Y, vScrollBar.ContentSize.Width, contentContainerHeight);
        }

        private class ContentContainer : Control
        {
            private readonly ScrollBar hScrollBar;
            private readonly ScrollBar vScrollBar;

            private Control content;
            private Point contentLocation;

            public ContentContainer(ScrollBar hScrollBar, ScrollBar vScrollBar)
            {
                this.hScrollBar = hScrollBar;
                this.vScrollBar = vScrollBar;
                RepaintMode = ControlRepaintMode.Never;
            }

            public Control Content
            {
                get { return content; }
                set
                {
                    if (content != value)
                    {
                        if (content != null)
                        {
                            RemoveChild(content);
                        }

                        content = value;

                        if (content != null)
                        {
                            AddChild(content);
                        }
                    }
                }
            }

            public Point ContentLocation
            {
                get { return contentLocation; }
                set
                {
                    if (contentLocation != value)
                    {
                        contentLocation = value;
                        InvalidateChildrenArea();
                    }
                }
            }

            protected override Size CalculateContentSize()
            {
                return Content?.ContentSize ?? Size.Empty;
            }

            protected override void PerformLayout()
            {
                if (Content != null)
                {
                    Content.Area = new Rectangle(Area.X + ContentLocation.X, Area.Y + ContentLocation.Y, Content.ContentSize.Width, Content.ContentSize.Height);
                }
            }

            protected override void OnAreaChanged()
            {
                UpdateScrollBars();
            }

            protected override void OnContentSizeChanged()
            {
                UpdateScrollBars();
            }

            private void UpdateScrollBars()
            {
                hScrollBar.SliderRange = Area.Width;
                vScrollBar.SliderRange = Area.Height;
                hScrollBar.MaxValue = ContentSize.Width - Area.Width;
                vScrollBar.MaxValue = ContentSize.Height - Area.Height;
                hScrollBar.Visibility = Area.Width < ContentSize.Width ? ControlVisibility.Visible : ControlVisibility.Collapsed;
                vScrollBar.Visibility = Area.Height < ContentSize.Height ? ControlVisibility.Visible : ControlVisibility.Collapsed;
            }
        }
    }
}