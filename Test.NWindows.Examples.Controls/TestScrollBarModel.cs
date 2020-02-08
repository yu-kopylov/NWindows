using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestScrollBarModel
    {
        [Test]
        public void Test()
        {
            var model = new ScrollBarModel();

            model.Min = 0;
            model.Max = 6;
            model.SliderValue = 0;
            model.SliderRange = 2;
            model.Size = 6;
            model.Padding = 0;
            model.MinSliderSize = 3;

            model.Calculate();

            Assert.That(model.GetValueFromSliderOffset(0), Is.EqualTo(0));
            Assert.That(model.GetValueFromSliderOffset(1), Is.EqualTo(2));
            Assert.That(model.GetValueFromSliderOffset(2), Is.EqualTo(4));
            Assert.That(model.GetValueFromSliderOffset(3), Is.EqualTo(6));

            Assert.That(model.SliderSize, Is.EqualTo(3));
            Assert.That(model.SliderOffset, Is.EqualTo(0));

            model.SliderValue = 6;
            model.Calculate();

            Assert.That(model.SliderSize, Is.EqualTo(3));
            Assert.That(model.SliderOffset, Is.EqualTo(3));

            model.Size = 8;
            model.Padding = 1;

            model.Calculate();

            Assert.That(model.GetValueFromSliderOffset(1), Is.EqualTo(0));
            Assert.That(model.GetValueFromSliderOffset(2), Is.EqualTo(2));
            Assert.That(model.GetValueFromSliderOffset(3), Is.EqualTo(4));
            Assert.That(model.GetValueFromSliderOffset(4), Is.EqualTo(6));

            Assert.That(model.SliderSize, Is.EqualTo(3));
            Assert.That(model.SliderOffset, Is.EqualTo(4));
        }
    }
}