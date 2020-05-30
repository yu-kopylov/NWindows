using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class ScrollBar : Control
    {
        private const int RailWidth = 10;
        private const int MinRailLength = 10;
        private const int Padding = 1;
        private const int BorderWidth = 1;

        private readonly ScrollBarCalculator calculator = new ScrollBarCalculator();
        private int value;
        private ScrollBarOrientation orientation = ScrollBarOrientation.Horizontal;

        public ScrollBar()
        {
            RepaintMode = ControlRepaintMode.Always;
            // todo: remove defaults
            calculator.MinValue = 0;
            calculator.MaxValue = 99;
            calculator.SliderRange = 10;
            calculator.MinSliderSize = 10;
            calculator.Padding = BorderWidth + Padding;
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
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    InvalidatePainting();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ValueChanged;

        public int MinValue
        {
            get { return calculator.MinValue; }
            set
            {
                if (calculator.MinValue != value)
                {
                    calculator.MinValue = value;
                    InvalidatePainting();
                }
            }
        }

        public int MaxValue
        {
            get { return calculator.MaxValue; }
            set
            {
                if (calculator.MaxValue != value)
                {
                    calculator.MaxValue = value;
                    InvalidatePainting();
                }
            }
        }

        public int SliderRange
        {
            get { return calculator.SliderRange; }
            set
            {
                if (calculator.SliderRange != value)
                {
                    calculator.SliderRange = value;
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
            calculator.Size = orientation == ScrollBarOrientation.Vertical ? Area.Height : Area.Width;

            int sliderSize = calculator.GetSliderSize();
            int sliderOffset = calculator.GetSliderOffsetFromValue(value);

            canvas.FillRectangle(Color.Black, 0, 0, Area.Width, Area.Height);
            canvas.FillRectangle(Color.White, BorderWidth, BorderWidth, Area.Width - 2 * BorderWidth, Area.Height - 2 * BorderWidth);
            if (orientation == ScrollBarOrientation.Vertical)
            {
                canvas.FillRectangle(Color.Blue, BorderWidth + Padding, sliderOffset, Area.Width - 2 * (BorderWidth + Padding), sliderSize);
            }
            else
            {
                canvas.FillRectangle(Color.Blue, sliderOffset, BorderWidth + Padding, sliderSize, Area.Height - 2 * (BorderWidth + Padding));
            }
        }

        protected override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            if (button == NMouseButton.Left)
            {
                int offset = GetOffsetFromPoint(point) - calculator.GetSliderSize() / 2;
                Value = calculator.GetValueFromSliderOffset(offset);
                CaptureMouse();
            }
        }

        protected override void OnMouseMove(Point point)
        {
            if (HasMouseCaptured)
            {
                int offset = GetOffsetFromPoint(point) - calculator.GetSliderSize() / 2;
                Value = calculator.GetValueFromSliderOffset(offset);
            }
        }

        protected override void OnMouseButtonUp(NMouseButton button, Point point)
        {
            if (button == NMouseButton.Left)
            {
                ReleaseMouse();
            }
        }

        private int GetOffsetFromPoint(Point point)
        {
            return orientation == ScrollBarOrientation.Vertical ? (point.Y - Area.Y) : (point.X - Area.X);
        }
    }
}