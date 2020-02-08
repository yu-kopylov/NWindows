using System;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class ScrollBarModel
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int SliderValue { get; set; }
        public int SliderRange { get; set; }
        public int Size { get; set; }
        public int Padding { get; set; }
        public int MinSliderSize { get; set; }

        public int SliderSize { get; private set; }
        public int SliderOffset { get; private set; }
        public int MinSliderOffset { get; private set; }
        public int MaxSliderOffset { get; private set; }

        public void Calculate()
        {
            int railSize = Size - 2 * Padding;
            int valuesRange = Max - Min + 1;

            SliderSize = Math.Max(MinSliderSize, (int) (railSize * (long) SliderRange / (Max - Min - 1 + SliderRange)));

            int valuesRange2 = Max - Min;
            int offsetRange2 = (Size - Padding - SliderSize) - (Padding);

            SliderOffset = Padding + (int) (offsetRange2 * (long) SliderValue / valuesRange2);
            MinSliderOffset = Padding;
            MaxSliderOffset = Size - Padding - SliderSize;
        }

        public int GetValueFromSliderOffset(int offset)
        {
            int valuesRange = Max - Min;
            int offsetRange = (Size - Padding - SliderSize) - (Padding);
            return (int) ((offset - Padding) * (long) valuesRange / offsetRange);
        }
    }
}