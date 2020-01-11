using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestFocus
    {
        [Test]
        public void Test()
        {
            var panel = new StackPanel();
            var textBox1 = new TextBox();
            var textBox2 = new TextBox();
            var textBox3 = new TextBox();
            panel.Add(textBox1);
            panel.Add(textBox2);
            panel.Add(textBox3);

            Window window = new Window();
            window.Content = panel;

            Assert.That(window.FocusedControl, Is.EqualTo(textBox1));

            window.MoveFocus(true);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox2));

            window.MoveFocus(true);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox3));

            window.MoveFocus(true);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox1));

            window.MoveFocus(false);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox3));

            window.MoveFocus(false);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox2));

            window.MoveFocus(false);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox1));

            panel.Remove(textBox1);

            // todo: it may not be a desired behaviour
            Assert.That(window.FocusedControl, Is.Null);

            window.MoveFocus(true);
            Assert.That(window.FocusedControl, Is.EqualTo(textBox2));
        }
    }
}