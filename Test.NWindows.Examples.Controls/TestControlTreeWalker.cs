using System.Collections.Generic;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestControlTreeWalker
    {
        [Test]
        public void Test()
        {
            var root = new StackPanel();
            var c1 = new StackPanel();
            var c2 = new StackPanel();
            var c11 = new StackPanel();
            var c12 = new StackPanel();
            var c21 = new StackPanel();
            var c22 = new StackPanel();

            root.Add(c1);
            root.Add(c2);
            c1.Add(c11);
            c1.Add(c12);
            c2.Add(c21);
            c2.Add(c22);

            List<Control> directOrder = new List<Control>();
            Control c = root;
            for (int i = 0; i < 8; i++, c = ControlTreeWalker.GetNextControl(c))
            {
                directOrder.Add(c);
            }

            Assert.That(directOrder.ToArray(), Is.EqualTo(new Control[] {root, c1, c11, c12, c2, c21, c22, root}));

            List<Control> reverseOrder = new List<Control>();
            c = root;
            for (int i = 0; i < 8; i++, c = ControlTreeWalker.GetPreviousControl(c))
            {
                reverseOrder.Add(c);
            }

            Assert.That(reverseOrder.ToArray(), Is.EqualTo(new Control[] {root, c22, c21, c2, c12, c11, c1, root}));
        }
    }
}