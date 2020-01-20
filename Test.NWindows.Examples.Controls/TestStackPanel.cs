using System.Drawing;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestStackPanel
    {
        [Test]
        public void TestEmpty()
        {
            var root = new StackPanel();
            root.Area = new Rectangle(100, 100, 2000, 1000);
            root.UpdateLayout();

            Assert.That(root.ContentSize, Is.EqualTo(Size.Empty));
            Assert.That(root.Area, Is.EqualTo(new Rectangle(100, 100, 2000, 1000)));
        }

        [Test]
        public void Test()
        {
            var root = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            var p1 = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            var p2 = new StackPanel {Orientation = StackPanelOrientation.Vertical};
            var textBox1 = new TextBox {PreferredSize = new Size(100, 20)};
            var textBox2 = new TextBox {PreferredSize = new Size(100, 20)};
            var textBox3 = new TextBox {PreferredSize = new Size(100, 20)};

            root.Add(p1);
            root.Add(p2);
            p1.Add(textBox1);
            p1.Add(textBox2);
            p2.Add(textBox3);

            root.Area = new Rectangle(100, 100, 2000, 1000);
            root.UpdateLayout();

            Assert.That(root.ContentSize, Is.EqualTo(new Size(200, 40)));
            Assert.That(root.Area, Is.EqualTo(new Rectangle(100, 100, 2000, 1000)));

            Assert.That(p1.ContentSize, Is.EqualTo(new Size(100, 40)));
            Assert.That(p1.Area, Is.EqualTo(new Rectangle(100, 100, 100, 1000)));

            Assert.That(p2.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(p2.Area, Is.EqualTo(new Rectangle(200, 100, 100, 1000)));

            Assert.That(textBox1.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox1.Area, Is.EqualTo(new Rectangle(100, 100, 100, 20)));

            Assert.That(textBox2.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox2.Area, Is.EqualTo(new Rectangle(100, 120, 100, 20)));

            Assert.That(textBox3.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox3.Area, Is.EqualTo(new Rectangle(200, 100, 100, 20)));

            textBox1.PreferredSize = new Size(150, 30);
            root.UpdateLayout();

            Assert.That(root.ContentSize, Is.EqualTo(new Size(250, 50)));
            Assert.That(root.Area, Is.EqualTo(new Rectangle(100, 100, 2000, 1000)));

            Assert.That(p1.ContentSize, Is.EqualTo(new Size(150, 50)));
            Assert.That(p1.Area, Is.EqualTo(new Rectangle(100, 100, 150, 1000)));

            Assert.That(p2.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(p2.Area, Is.EqualTo(new Rectangle(250, 100, 100, 1000)));

            Assert.That(textBox1.ContentSize, Is.EqualTo(new Size(150, 30)));
            Assert.That(textBox1.Area, Is.EqualTo(new Rectangle(100, 100, 150, 30)));

            Assert.That(textBox2.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox2.Area, Is.EqualTo(new Rectangle(100, 130, 150, 20)));

            Assert.That(textBox3.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox3.Area, Is.EqualTo(new Rectangle(250, 100, 100, 20)));

            p1.Orientation = StackPanelOrientation.Horizontal;
            root.UpdateLayout();

            Assert.That(root.ContentSize, Is.EqualTo(new Size(350, 30)));
            Assert.That(root.Area, Is.EqualTo(new Rectangle(100, 100, 2000, 1000)));

            Assert.That(p1.ContentSize, Is.EqualTo(new Size(250, 30)));
            Assert.That(p1.Area, Is.EqualTo(new Rectangle(100, 100, 250, 1000)));

            Assert.That(p2.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(p2.Area, Is.EqualTo(new Rectangle(350, 100, 100, 1000)));

            Assert.That(textBox1.ContentSize, Is.EqualTo(new Size(150, 30)));
            Assert.That(textBox1.Area, Is.EqualTo(new Rectangle(100, 100, 150, 1000)));

            Assert.That(textBox2.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox2.Area, Is.EqualTo(new Rectangle(250, 100, 100, 1000)));

            Assert.That(textBox3.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox3.Area, Is.EqualTo(new Rectangle(350, 100, 100, 20)));
        }

        [Test]
        public void TestOrientationChanged()
        {
            var root = new StackPanel {Orientation = StackPanelOrientation.Horizontal};
            var textBox = new TextBox {PreferredSize = new Size(100, 20)};

            root.Area = new Rectangle(0, 0, 2000, 1000);
            root.Add(textBox);

            root.UpdateLayout();

            Assert.That(root.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(root.Area, Is.EqualTo(new Rectangle(0, 0, 2000, 1000)));

            Assert.That(textBox.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox.Area, Is.EqualTo(new Rectangle(0, 0, 100, 1000)));

            root.Orientation = StackPanelOrientation.Vertical;
            root.UpdateLayout();

            Assert.That(root.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(root.Area, Is.EqualTo(new Rectangle(0, 0, 2000, 1000)));

            Assert.That(textBox.ContentSize, Is.EqualTo(new Size(100, 20)));
            Assert.That(textBox.Area, Is.EqualTo(new Rectangle(0, 0, 2000, 20)));
        }
    }
}