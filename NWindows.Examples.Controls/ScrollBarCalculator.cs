using System;

namespace NWindows.Examples.Controls
{
    public class ScrollBarCalculator
    {
        private int minValue;
        private int maxValue;
        private int size;
        private int padding;
        private int sliderRange;
        private int minSliderSize;

        private bool parametersReady;
        private int railSize;
        private int valuesRange;
        private int sliderSize;
        private int offsetRange;
        private int minSliderOffset;
        private int maxSliderOffset;

        public int MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                parametersReady = false;
            }
        }

        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                parametersReady = false;
            }
        }

        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                parametersReady = false;
            }
        }

        public int Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                parametersReady = false;
            }
        }

        public int SliderRange
        {
            get { return sliderRange; }
            set
            {
                sliderRange = value;
                parametersReady = false;
            }
        }

        public int MinSliderSize
        {
            get { return minSliderSize; }
            set
            {
                minSliderSize = value;
                parametersReady = false;
            }
        }

        public int GetSliderSize()
        {
            CalculateParameters();
            return sliderSize;
        }

        public int GetSliderOffsetFromValue(int value)
        {
            CalculateParameters();

            if (value <= minValue)
            {
                return minSliderOffset;
            }

            if (value >= maxValue)
            {
                return maxSliderOffset;
            }

            return padding + (int) ((offsetRange * (long) (value - minValue) + valuesRange / 2) / valuesRange);
        }

        public int GetValueFromSliderOffset(int offset)
        {
            CalculateParameters();

            if (offset <= minSliderOffset)
            {
                return minValue;
            }

            if (offset >= maxSliderOffset)
            {
                return maxValue;
            }

            return minValue + (int) (((offset - padding) * (long) valuesRange + offsetRange / 2) / offsetRange);
        }

        private void CalculateParameters()
        {
            if (parametersReady)
            {
                return;
            }

            parametersReady = true;

            railSize = Math.Max(0, size - 2 * padding);
            valuesRange = Math.Max(0, maxValue - minValue);
            int totalValuesRange = valuesRange + Math.Max(sliderRange, 0);

            if (totalValuesRange == 0)
            {
                sliderSize = railSize;
            }
            else
            {
                sliderSize = Math.Min(railSize, Math.Max(minSliderSize, (int) ((railSize * (long) sliderRange + totalValuesRange / 2) / totalValuesRange)));
            }

            offsetRange = railSize - sliderSize;

            minSliderOffset = padding;
            maxSliderOffset = padding + offsetRange;
        }
    }
}