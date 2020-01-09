using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestLinkedHashSet
    {
        [Test]
        public void TestAddRemove()
        {
            var set = new LinkedHashSet<int>(EqualityComparer<int>.Default);
            Assert.That(set.Add(11), Is.True);
            Assert.That(set.Add(21), Is.True);
            Assert.That(set.Add(31), Is.True);

            Assert.That(set.Count, Is.EqualTo(3));
            Assert.That(set.ToArray(), Is.EqualTo(new int[] {11, 21, 31}));

            Assert.That(set.Add(31), Is.False);
            Assert.That(set.Add(21), Is.False);
            Assert.That(set.Add(11), Is.False);

            Assert.That(set.Count, Is.EqualTo(3));
            Assert.That(set.ToArray(), Is.EqualTo(new int[] {11, 21, 31}));

            Assert.That(set.Add(42), Is.True);

            Assert.That(set.Count, Is.EqualTo(4));
            Assert.That(set.ToArray(), Is.EqualTo(new int[] {11, 21, 31, 42}));

            Assert.That(set.Remove(21), Is.True);
            Assert.That(set.Remove(31), Is.True);
            Assert.That(set.Remove(52), Is.False);

            Assert.That(set.Count, Is.EqualTo(2));
            Assert.That(set.ToArray(), Is.EqualTo(new int[] {11, 42}));

            Assert.That(set.Add(13), Is.True);
            Assert.That(set.Add(23), Is.True);

            Assert.That(set.Count, Is.EqualTo(4));
            Assert.That(set.ToArray(), Is.EqualTo(new int[] {11, 42, 13, 23}));
        }

        [Test]
        public void TestEqualityComparer()
        {
            var set = new LinkedHashSet<int>(AlwaysEqualEqualityComparer<int>.Instance);

            Assert.That(set.Add(10), Is.True);
            Assert.That(set.Add(20), Is.False);
            Assert.That(set.Add(30), Is.False);

            Assert.That(set.Count, Is.EqualTo(1));

            Assert.That(set.Remove(40), Is.True);
            Assert.That(set.Remove(50), Is.False);
            Assert.That(set.Remove(10), Is.False);

            Assert.That(set.Count, Is.EqualTo(0));
        }

        private class AlwaysEqualEqualityComparer<T> : IEqualityComparer<T>
        {
            public static AlwaysEqualEqualityComparer<T> Instance { get; } = new AlwaysEqualEqualityComparer<T>();

            public bool Equals(T x, T y)
            {
                return true;
            }

            public int GetHashCode(T obj)
            {
                return 0;
            }
        }
    }
}