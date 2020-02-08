using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class ScrollBar : Control
    {
        private readonly ScrollBarModel model = new ScrollBarModel();
        private ScrollBarOrientation orientation = ScrollBarOrientation.Horizontal;

        private const int RailWidth = 10;
        private const int MinRailLength = 10;
        private const int Padding = 1;
        private const int BorderWidth = 1;

        public ScrollBar()
        {
            RepaintMode = ControlRepaintMode.Always;
            // todo: remove defaults
            model.Min = 0;
            model.Max = 99;
            model.SliderValue = 33;
            model.SliderRange = 10;
            model.MinSliderSize = 10;
            model.Padding = BorderWidth + Padding;
        }

        public ScrollBarOrientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    InvalidateContentSize();
                }
            }
        }

        public int Value
        {
            get { return model.SliderValue; }
            set
            {
                if (model.SliderValue != value)
                {
                    model.SliderValue = value;
                    InvalidatePainting();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ValueChanged;

        public int MinValue
        {
            get { return model.Min; }
            set
            {
                if (model.Min != value)
                {
                    model.Min = value;
                    InvalidatePainting();
                }
            }
        }

        public int MaxValue
        {
            get { return model.Max; }
            set
            {
                if (model.Max != value)
                {
                    model.Max = value;
                    InvalidatePainting();
                }
            }
        }

        public int SliderRange
        {
            get { return model.SliderRange; }
            set
            {
                if (model.SliderRange != value)
                {
                    model.SliderRange = value;
                    InvalidatePainting();
                }
            }
        }

        protected override Size CalculateContentSize()
        {
            if (orientation == ScrollBarOrientation.Vertical)
            {
                return new Size(2 * (BorderWidth + Padding) + RailWidth, 2 * (BorderWidth + Padding) + MinRailLength);
            }
            else
            {
                return new Size(2 * (BorderWidth + Padding) + MinRailLength, 2 * (BorderWidth + Padding) + RailWidth);
            }
        }

        protected override void OnPaint(ICanvas canvas, Rectangle area)
        {
            model.Size = orientation == ScrollBarOrientation.Vertical ? Area.Height : Area.Width;
            model.Calculate();

            canvas.FillRectangle(Color.Black, 0, 0, Area.Width, Area.Height);
            canvas.FillRectangle(Color.White, BorderWidth, BorderWidth, Area.Width - 2 * BorderWidth, Area.Height - 2 * BorderWidth);
            if (orientation == ScrollBarOrientation.Vertical)
            {
                canvas.FillRectangle(Color.Blue, BorderWidth + Padding, model.SliderOffset, Area.Width - 2 * (BorderWidth + Padding), model.SliderSize);
            }
            else
            {
                canvas.FillRectangle(Color.Blue, model.SliderOffset, BorderWidth + Padding, model.SliderSize, Area.Height - 2 * (BorderWidth + Padding));
            }
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            if (button == NMouseButton.Left)
            {
                int offset = GetOffsetFromPoint(point);
                Value = LimitRange(model.GetValueFromSliderOffset(offset) - model.SliderRange / 2);
                CaptureMouse();
            }
        }

        protected override void OnMouseMove(Point point)
        {
            if (HasMouseCaptured)
            {
                int offset = GetOffsetFromPoint(point);
                Value = LimitRange(model.GetValueFromSliderOffset(offset) - model.SliderRange / 2);
            }
        }

        protected override void OnMouseButtonUp(NMouseButton button, Point point)
        {
            if (button == NMouseButton.Left)
            {
                ReleaseMouse();
            }
        }

        private int LimitRange(int value)
        {
            if (value < MinValue)
            {
                return MinValue;
            }

            if (value > MaxValue)
            {
                return MaxValue;
            }

            return value;
        }

        private int GetOffsetFromPoint(Point point)
        {
            return orientation == ScrollBarOrientation.Vertical ? (point.Y - Area.Y) : (point.X - Area.X);
        }
    }
}