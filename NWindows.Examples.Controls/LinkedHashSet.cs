using System.Collections;
using System.Collections.Generic;

namespace NWindows.Examples.Controls
{
    public class LinkedHashSet<T> : IReadOnlyCollection<T>
    {
        private readonly LinkedList<T> nodes;
        private readonly Dictionary<T, LinkedListNode<T>> nodesByValue;

        public LinkedHashSet(IEqualityComparer<T> comparer)
        {
            nodes = new LinkedList<T>();
            nodesByValue = new Dictionary<T, LinkedListNode<T>>(comparer);
        }

        public bool Add(T value)
        {
            if (nodesByValue.ContainsKey(value))
            {
                return false;
            }

            nodesByValue.Add(value, nodes.AddLast(value));
            return true;
        }

        public bool Remove(T value)
        {
            if (!nodesByValue.TryGetValue(value, out var node))
            {
                return false;
            }

            nodesByValue.Remove(value);
            nodes.Remove(node);
            return true;
        }

        public int Count
        {
            get { return nodes.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}