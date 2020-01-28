using System;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestControl
    {
        [Test]
        public void TestLoopDetection()
        {
            var c1 = new StackPanel();
            var c2 = new StackPanel();
            var c3 = new StackPanel();

            c1.Add(c2);
            c2.Add(c3);

            Assert.That(() => c1.Add(c1), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("Control cannot contain itself or its own parent."));
            Assert.That(() => c2.Add(c1), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("Control cannot contain itself or its own parent."));
            Assert.That(() => c3.Add(c1), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("Control cannot contain itself or its own parent."));

            Assert.That(() => c1.Add(c2), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control already has parent."));
            Assert.That(() => c1.Add(c3), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control already has parent."));
            Assert.That(() => c2.Add(c2), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control already has parent."));
            Assert.That(() => c2.Add(c3), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control already has parent."));
        }

        [Test]
        public void TestRemoveChildValidation()
        {
            var c1 = new StackPanel();
            var c2 = new StackPanel();
            var c3 = new StackPanel();

            c1.Add(c2);
            c2.Add(c3);

            Assert.That(() => c1.Remove(c1), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));
            Assert.That(() => c1.Remove(c3), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));

            Assert.That(() => c2.Remove(c1), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));
            Assert.That(() => c2.Remove(c2), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));

            Assert.That(() => c3.Remove(c1), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));
            Assert.That(() => c3.Remove(c2), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));
            Assert.That(() => c3.Remove(c3), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));

            c1.Remove(c2);
            c2.Remove(c3);

            Assert.That(() => c1.Remove(c2), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));
            Assert.That(() => c2.Remove(c3), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("The given control is not a child of this control."));
        }

        [Test]
        public void TestVisibilityCascade()
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