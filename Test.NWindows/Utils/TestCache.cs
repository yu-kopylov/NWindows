using System;
using System.Collections.Generic;
using NUnit.Framework;
using NWindows.Utils;

namespace Test.NWindows.Utils
{
    public class TestCache
    {
        [Test]
        public void TestGetEvict()
        {
            List<string> evicted = new List<string>();
            var cache = new Cache<int, string>(5, i => i.ToString(), s => evicted.Add(s), EqualityComparer<int>.Default);

            Assert.That(cache.Get(1), Is.EqualTo("1"));
            Assert.That(cache.Get(2), Is.EqualTo("2"));
            Assert.That(cache.Get(3), Is.EqualTo("3"));
            Assert.That(cache.Get(4), Is.EqualTo("4"));
            Assert.That(cache.Get(5), Is.EqualTo("5"));
            Assert.That(evicted, Is.Empty);

            Assert.That(cache.Get(6), Is.EqualTo("6"));
            Assert.That(evicted, Is.EqualTo(new string[] {"1"}));

            Assert.That(cache.Get(2), Is.EqualTo("2"));
            Assert.That(evicted, Is.EqualTo(new string[] {"1"}));

            Assert.That(cache.Get(7), Is.EqualTo("7"));
            Assert.That(evicted, Is.EqualTo(new string[] {"1", "3"}));

            cache.Clear();
            Assert.That(evicted, Is.EquivalentTo(new string[] {"1", "2", "3", "4", "5", "6", "7"}));
            evicted.Clear();

            Assert.That(cache.Get(11), Is.EqualTo("11"));
            Assert.That(cache.Get(12), Is.EqualTo("12"));
            Assert.That(cache.Get(13), Is.EqualTo("13"));
            Assert.That(cache.Get(14), Is.EqualTo("14"));
            Assert.That(cache.Get(15), Is.EqualTo("15"));
            Assert.That(evicted, Is.Empty);

            Assert.That(cache.Get(15), Is.EqualTo("15"));
            Assert.That(cache.Get(14), Is.EqualTo("14"));
            Assert.That(cache.Get(13), Is.EqualTo("13"));
            Assert.That(cache.Get(12), Is.EqualTo("12"));
            Assert.That(cache.Get(11), Is.EqualTo("11"));
            Assert.That(evicted, Is.Empty);

            Assert.That(cache.Get(21), Is.EqualTo("21"));
            Assert.That(cache.Get(22), Is.EqualTo("22"));
            Assert.That(cache.Get(23), Is.EqualTo("23"));
            Assert.That(cache.Get(24), Is.EqualTo("24"));
            Assert.That(cache.Get(25), Is.EqualTo("25"));
            Assert.That(evicted, Is.EqualTo(new string[] {"15", "14", "13", "12", "11"}));
        }

        [Test]
        public void TestExceptionInConstructor()
        {
            string ValueConstructor(int i)
            {
                if (i % 2 == 0)
                {
                    throw new InvalidOperationException();
                }

                return i.ToString();
            }

            List<string> evicted = new List<string>();
            var cache = new Cache<int, string>(3, ValueConstructor, s => evicted.Add(s), EqualityComparer<int>.Default);

            Assert.That(cache.Get(1), Is.EqualTo("1"));
            Assert.That(() => cache.Get(2), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(() => cache.Get(2), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(cache.Get(3), Is.EqualTo("3"));
            Assert.That(() => cache.Get(4), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(() => cache.Get(4), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(cache.Get(5), Is.EqualTo("5"));
            Assert.That(() => cache.Get(2), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(() => cache.Get(4), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(() => cache.Get(6), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(evicted, Is.Empty);

            Assert.That(cache.Get(7), Is.EqualTo("7"));
            Assert.That(evicted, Is.EqualTo(new string[] {"1"}));

            cache.Clear();
            Assert.That(evicted, Is.EquivalentTo(new string[] {"1", "3", "5", "7"}));
            evicted.Clear();

            cache.Clear();
            Assert.That(evicted, Is.Empty);
        }

        [Test]
        public void TestEqualityComparer()
        {
            List<string> evicted = new List<string>();
            var cache = new Cache<int, string>(3, i => i.ToString(), s => evicted.Add(s), Mod10EqualityComparer.Instance);

            Assert.That(cache.Get(11), Is.EqualTo("11"));
            Assert.That(cache.Get(12), Is.EqualTo("12"));
            Assert.That(cache.Get(13), Is.EqualTo("13"));
            Assert.That(evicted, Is.Empty);

            Assert.That(cache.Get(21), Is.EqualTo("11"));
            Assert.That(cache.Get(22), Is.EqualTo("12"));
            Assert.That(cache.Get(23), Is.EqualTo("13"));
            Assert.That(evicted, Is.Empty);

            Assert.That(cache.Get(15), Is.EqualTo("15"));
            Assert.That(evicted, Is.EqualTo(new string[] {"11"}));

            cache.Clear();
            Assert.That(evicted, Is.EquivalentTo(new string[] {"11", "12", "13", "15"}));
        }

        private class Mod10EqualityComparer : IEqualityComparer<int>
        {
            public static Mod10EqualityComparer Instance { get; } = new Mod10EqualityComparer();

            private Mod10EqualityComparer() {}

            public bool Equals(int x, int y)
            {
                return x % 10 == y % 10;
            }

            public int GetHashCode(int obj)
            {
                return obj % 10;
            }
        }
    }
}