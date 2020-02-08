using System.Drawing;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestRectangleExtensions
    {
        [Test]
        public void TestExclude()
        {
            var rect1 = new Rectangle(0, 0, 200, 100);
            var rect2 = new Rectangle(200, 100, 200, 100);
            var rect3 = new Rectangle(10, 20, 200, 100);
            var rect4 = new Rectangle(30, 30, 30, 30);

            Assert.That(RectangleExtensions.Exclude(rect1, rect1), Is.Empty);
            Assert.That(RectangleExtensions.Exclude(rect1, rect2), Is.EquivalentTo(new Rectangle[] {rect1}));
            Assert.That(RectangleExtensions.Exclude(rect2, rect1), Is.EquivalentTo(new Rectangle[] {rect2}));

            Assert.That(RectangleExtensions.Exclude(rect1, rect3), Is.EquivalentTo(new Rectangle[]
            {
                new Rectangle(0, 0, 200, 20),
                new Rectangle(0, 20, 10, 80)
            }));

            Assert.That(RectangleExtensions.Exclude(rect2, rect3), Is.EquivalentTo(new Rectangle[]
            {
                new Rectangle(200, 120, 200, 80),
                new Rectangle(210, 100, 190, 20)
            }));

            Assert.That(RectangleExtensions.Exclude(rect1, rect4), Is.EquivalentTo(new Rectangle[]
            {
                new Rectangle(0, 0, 200, 30),
                new Rectangle(0, 60, 200, 40),
                new Rectangle(0, 30, 30, 30),
                new Rectangle(60, 30, 140, 30)
            }));

            Assert.That(RectangleExtensions.Exclude(rect2, rect4), Is.EquivalentTo(new Rectangle[] {rect2}));
        }
    }
}