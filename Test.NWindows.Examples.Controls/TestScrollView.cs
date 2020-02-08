using System.Drawing;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestScrollView
    {
        [Test]
        public void Test()
        {
            var scrollView = new ScrollView {Area = new Rectangle(0, 0, 100, 100)};
            var content = new TextBox {PreferredSize = new Size(200, 200)};
            scrollView.Content = content;
            scrollView.Update();
            Assert.That(content.Area, Is.EqualTo(new Rectangle(0, 0, 200, 200)));
            Assert.That(content.VisibleArea.Location, Is.EqualTo(new Point(0, 0)));

            var xPadding = scrollView.Area.Width - content.VisibleArea.Width;
            var yPadding = scrollView.Area.Height - content.VisibleArea.Height;

            Assert.That(xPadding, Is.GreaterThan(0).And.LessThan(scrollView.Area.Width));
            Assert.That(yPadding, Is.GreaterThan(0).And.LessThan(scrollView.Area.Height));

            scrollView.Area = new Rectangle(0, 0, 120 + xPadding, 130 + yPadding);
            scrollView.Update();

            Assert.That(content.Area, Is.EqualTo(new Rectangle(0, 0, 200, 200)));
            Assert.That(content.VisibleArea, Is.EqualTo(new Rectangle(0, 0, 120, 130)));
        }
    }
}