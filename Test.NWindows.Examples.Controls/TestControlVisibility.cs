using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestControlVisibility
    {
        [Test]
        public void TestCascade()
        {
            var p1 = new StackPanel();
            var p2 = new StackPanel();
            var p3 = new StackPanel();
            var p4 = new StackPanel();
            var p5 = new StackPanel();

            p1.Add(p2);
            p2.Add(p3);
            p3.Add(p4);

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));

            p3.Visibility = ControlVisibility.Hidden;

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Hidden));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Hidden));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));

            p2.Visibility = ControlVisibility.Collapsed;

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));

            p1.Visibility = ControlVisibility.Collapsed;

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));

            p4.Add(p5);

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));

            p1.Remove(p2);

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));

            p2.Remove(p3);

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Hidden));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Hidden));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Hidden));

            p3.Remove(p4);

            Assert.That(p1.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p2.EffectiveVisibility, Is.EqualTo(ControlVisibility.Collapsed));
            Assert.That(p3.EffectiveVisibility, Is.EqualTo(ControlVisibility.Hidden));
            Assert.That(p4.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
            Assert.That(p5.EffectiveVisibility, Is.EqualTo(ControlVisibility.Visible));
        }
    }
}