using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestScrollBarCalculator
    {
        [Test]
        public void TestSliderSize([Values(0, 10)] int minValue, [Values(0, 10)] int padding)
        {
            var model = new ScrollBarCalculator();

            model.MinValue = minValue + 0;
            model.MaxValue = minValue + 50;
            model.Size = 100 + 2 * padding;
            model.Padding = padding;
            model.SliderRange = 50;
            model.MinSliderSize = 3;

            Assert.That(model.GetSliderSize(), Is.EqualTo(50));
        }

        [Test]
        public void Test_7Values_4Offsets([Values(0, 1)] int minValue, [Values(0, 1)] int padding)
        {
            var model = new ScrollBarCalculator();

            model.MinValue = minValue + 0;
            model.MaxValue = minValue + 6;
            model.Size = 6 + 2 * padding;
            model.Padding = padding;
            model.SliderRange = 2;
            model.MinSliderSize = 3;

            Assert.That(model.GetSliderSize(), Is.EqualTo(3));

            Assert.That(model.GetValueFromSliderOffset(padding - 1), Is.EqualTo(minValue + 0));
            Assert.That(model.GetValueFromSliderOffset(padding + 0), Is.EqualTo(minValue + 0));
            Assert.That(model.GetValueFromSliderOffset(padding + 1), Is.EqualTo(minValue + 2));
            Assert.That(model.GetValueFromSliderOffset(padding + 2), Is.EqualTo(minValue + 4));
            Assert.That(model.GetValueFromSliderOffset(padding + 3), Is.EqualTo(minValue + 6));
            Assert.That(model.GetValueFromSliderOffset(padding + 4), Is.EqualTo(minValue + 6));

            Assert.That(model.GetSliderOffsetFromValue(minValue - 1), Is.EqualTo(padding + 0));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 0), Is.EqualTo(padding + 0));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 2), Is.EqualTo(padding + 1));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 4), Is.EqualTo(padding + 2));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 6), Is.EqualTo(padding + 3));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 7), Is.EqualTo(padding + 3));
        }

        [Test]
        public void Test_4Values_7Offsets([Values(0, 1)] int minValue, [Values(0, 1)] int padding)
        {
            var model = new ScrollBarCalculator();

            model.MinValue = minValue + 0;
            model.MaxValue = minValue + 3;
            model.Size = 9 + 2 * padding;
            model.Padding = padding;
            model.SliderRange = 1;
            model.MinSliderSize = 3;

            Assert.That(model.GetSliderSize(), Is.EqualTo(3));

            Assert.That(model.GetValueFromSliderOffset(padding - 1), Is.EqualTo(minValue + 0));
            Assert.That(model.GetValueFromSliderOffset(padding + 0), Is.EqualTo(minValue + 0));
            Assert.That(model.GetValueFromSliderOffset(padding + 2), Is.EqualTo(minValue + 1));
            Assert.That(model.GetValueFromSliderOffset(padding + 4), Is.EqualTo(minValue + 2));
            Assert.That(model.GetValueFromSliderOffset(padding + 6), Is.EqualTo(minValue + 3));
            Assert.That(model.GetValueFromSliderOffset(padding + 7), Is.EqualTo(minValue + 3));

            Assert.That(model.GetSliderOffsetFromValue(minValue - 1), Is.EqualTo(padding + 0));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 0), Is.EqualTo(padding + 0));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 1), Is.EqualTo(padding + 2));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 2), Is.EqualTo(padding + 4));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 3), Is.EqualTo(padding + 6));
            Assert.That(model.GetSliderOffsetFromValue(minValue + 4), Is.EqualTo(padding + 6));
        }

        [Test]
        public void TestZeroValuesRange()
        {
            var model = new ScrollBarCalculator();

            model.MinValue = 10;
            model.MaxValue = 10;
            model.Size = 120;
            model.Padding = 10;
            model.MinSliderSize = 0;

            model.SliderRange = 10;
            Assert.That(model.GetSliderSize(), Is.EqualTo(100));
            Assert.That(model.GetValueFromSliderOffset(10), Is.EqualTo(10));
            Assert.That(model.GetSliderOffsetFromValue(10), Is.EqualTo(10));

            model.SliderRange = 0;
            Assert.That(model.GetSliderSize(), Is.EqualTo(100));
            Assert.That(model.GetValueFromSliderOffset(10), Is.EqualTo(10));
            Assert.That(model.GetSliderOffsetFromValue(10), Is.EqualTo(10));

            model.SliderRange = -10;
            Assert.That(model.GetSliderSize(), Is.EqualTo(100));
            Assert.That(model.GetValueFromSliderOffset(10), Is.EqualTo(10));
            Assert.That(model.GetSliderOffsetFromValue(10), Is.EqualTo(10));
        }

        [Test]
        public void TestZeroOffsetRange()
        {
            var model = new ScrollBarCalculator();

            model.MinValue = 10;
            model.MaxValue = 20;
            model.Size = 120;
            model.Padding = 10;
            model.MinSliderSize = 200;

            model.SliderRange = 10;
            Assert.That(model.GetSliderSize(), Is.EqualTo(100));
            Assert.That(model.GetValueFromSliderOffset(10), Is.EqualTo(10));
            Assert.That(model.GetSliderOffsetFromValue(15), Is.EqualTo(10));
        }
    }
}