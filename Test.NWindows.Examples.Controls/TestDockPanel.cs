using System.Drawing;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestDockPanel
    {
        [Test]
        public void TestLayout()
        {
            var panel = new DockPanel();
            panel.Top = new StackPanel {PreferredSize = new Size(10, 20)};
            panel.Bottom = new StackPanel {PreferredSize = new Size(20, 30)};
            panel.Left = new StackPanel {PreferredSize = new Size(30, 40)};
            panel.Right = new StackPanel {PreferredSize = new Size(40, 50)};
            panel.Center = new StackPanel {PreferredSize = new Size(50, 60)};

            panel.Area = new Rectangle(1, 2, 300, 200);
            panel.Update();

            Assert.That(panel.ContentSize, Is.EqualTo(new Size(120, 110)));
            Assert.That(panel.Area, Is.EqualTo(new Rectangle(1, 2, 300, 200)));
            Assert.That(panel.Top.Area, Is.EqualTo(new Rectangle(1, 2, 300, 20)));
            Assert.That(panel.Bottom.Area, Is.EqualTo(new Rectangle(1, 172, 300, 30)));
            Assert.That(panel.Left.Area, Is.EqualTo(new Rectangle(1, 22, 30, 150)));
            Assert.That(panel.Right.Area, Is.EqualTo(new Rectangle(261, 22, 40, 150)));
            Assert.That(panel.Center.Area, Is.EqualTo(new Rectangle(31, 22, 230, 150)));

            panel.Area = new Rectangle(2, 3, 25, 15);
            panel.Update();

            Assert.That(panel.ContentSize, Is.EqualTo(new Size(120, 110)));
            Assert.That(panel.Area, Is.EqualTo(new Rectangle(2, 3, 25, 15)));
            Assert.That(panel.Top.Area, Is.EqualTo(new Rectangle(2, 3, 25, 15)));
            Assert.That(panel.Bottom.Area, Is.EqualTo(new Rectangle(2, 3, 25, 15)));
            Assert.That(panel.Left.Area, Is.EqualTo(Rectangle.Empty));
            Assert.That(panel.Right.Area, Is.EqualTo(Rectangle.Empty));
            Assert.That(panel.Center.Area, Is.EqualTo(Rectangle.Empty));

            panel.Area = new Rectangle(3, 4, 25, 90);
            panel.Update();

            Assert.That(panel.ContentSize, Is.EqualTo(new Size(120, 110)));
            Assert.That(panel.Area, Is.EqualTo(new Rectangle(3, 4, 25, 90)));
            Assert.That(panel.Top.Area, Is.EqualTo(new Rectangle(3, 4, 25, 20)));
            Assert.That(panel.Bottom.Area, Is.EqualTo(new Rectangle(3, 64, 25, 30)));
            Assert.That(panel.Left.Area, Is.EqualTo(new Rectangle(3, 24, 25, 40)));
            Assert.That(panel.Right.Area, Is.EqualTo(new Rectangle(3, 24, 25, 40)));
            Assert.That(panel.Center.Area, Is.EqualTo(Rectangle.Empty));

            panel.Top = null;
            panel.Bottom = null;
            panel.Update();

            Assert.That(panel.ContentSize, Is.EqualTo(new Size(120, 60)));
            Assert.That(panel.Area, Is.EqualTo(new Rectangle(3, 4, 25, 90)));
            Assert.That(panel.Top, Is.Null);
            Assert.That(panel.Bottom, Is.Null);
            Assert.That(panel.Left.Area, Is.EqualTo(new Rectangle(3, 4, 25, 90)));
            Assert.That(panel.Right.Area, Is.EqualTo(new Rectangle(3, 4, 25, 90)));
            Assert.That(panel.Center.Area, Is.EqualTo(Rectangle.Empty));

            panel.Left = null;
            panel.Right = null;
            panel.Update();

            Assert.That(panel.ContentSize, Is.EqualTo(new Size(50, 60)));
            Assert.That(panel.Area, Is.EqualTo(new Rectangle(3, 4, 25, 90)));
            Assert.That(panel.Top, Is.Null);
            Assert.That(panel.Bottom, Is.Null);
            Assert.That(panel.Left, Is.Null);
            Assert.That(panel.Right, Is.Null);
            Assert.That(panel.Center.Area, Is.EqualTo(new Rectangle(3, 4, 25, 90)));
        }
    }
}