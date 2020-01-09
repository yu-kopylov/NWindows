using NUnit.Framework;
using NWindows.Examples.Controls;

namespace Test.NWindows.Examples.Controls
{
    public class TestReferenceEqualityComparer
    {
        [Test]
        public void Test()
        {
            string s1 = "1";
            string s2 = "2";
            string s12 = "12";
            string s1_2a = Concat(s1, s2);
            string s1_2b = Concat(s1, s2);
            string s1_2c = Concat(s1, s2);

            Assert.That(s12, Is.EqualTo(s1_2a), "sanity check");
            Assert.That(s12, Is.EqualTo(s1_2b), "sanity check");
            Assert.That(s12, Is.EqualTo(s1_2c), "sanity check");

            var comparer = ReferenceEqualityComparer<string>.Instance;
            Assert.That(comparer, Is.SameAs(ReferenceEqualityComparer<string>.Instance));

            Assert.That(comparer.Equals(s1, s1), Is.True);
            Assert.That(comparer.Equals(s2, s2), Is.True);
            Assert.That(comparer.Equals(s12, s12), Is.True);
            Assert.That(comparer.Equals(s1_2a, s1_2a), Is.True);
            Assert.That(comparer.Equals(s1_2b, s1_2b), Is.True);
            Assert.That(comparer.Equals(s1_2c, s1_2c), Is.True);

            Assert.That(comparer.Equals(s12, s1_2a), Is.False);
            Assert.That(comparer.Equals(s12, s1_2b), Is.False);
            Assert.That(comparer.Equals(s12, s1_2c), Is.False);
            Assert.That(comparer.Equals(s1_2a, s1_2b), Is.False);
            Assert.That(comparer.Equals(s1_2a, s1_2c), Is.False);
            Assert.That(comparer.Equals(s1_2b, s1_2c), Is.False);

            Assert.That(comparer.GetHashCode(s1), Is.EqualTo(comparer.GetHashCode(s1)));
            Assert.That(comparer.GetHashCode(s2), Is.EqualTo(comparer.GetHashCode(s2)));
            Assert.That(comparer.GetHashCode(s12), Is.EqualTo(comparer.GetHashCode(s12)));
            Assert.That(comparer.GetHashCode(s1_2a), Is.EqualTo(comparer.GetHashCode(s1_2a)));
            Assert.That(comparer.GetHashCode(s1_2b), Is.EqualTo(comparer.GetHashCode(s1_2b)));
            Assert.That(comparer.GetHashCode(s1_2c), Is.EqualTo(comparer.GetHashCode(s1_2c)));

            Assert.That
            (
                comparer.GetHashCode(s12) != comparer.GetHashCode(s1_2a) ||
                comparer.GetHashCode(s12) != comparer.GetHashCode(s1_2b) ||
                comparer.GetHashCode(s12) != comparer.GetHashCode(s1_2c),
                "This condition is not guaranteed, but it is highly unlikely to fail with correct implementation of GetHashCode."
            );
        }

        private static string Concat(string a, string b)
        {
            return a + b;
        }
    }
}