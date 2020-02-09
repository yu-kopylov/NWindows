using System;
using System.Collections.Generic;

namespace NWindows.Utils
{
    internal class Cache<K, V>
    {
        private readonly Dictionary<K, LinkedListNode<(K, V)>> nodesByKey;
        private readonly LinkedList<(K, V)> nodes;

        private readonly int size;
        private readonly Func<K, V> valueConstructor;
        private readonly Action<V> evictionCallback;

        public Cache(int size, Func<K, V> valueConstructor, Action<V> evictionCallback, IEqualityComparer<K> comparer)
        {
            nodesByKey = new Dictionary<K, LinkedListNode<(K, V)>>(comparer);
            nodes = new LinkedList<(K, V)>();

            this.size = size;
            this.valueConstructor = valueConstructor;
            this.evictionCallback = evictionCallback;
        }

        public void Clear()
        {
            foreach (var value in nodes)
            {
                evictionCallback(value.Item2);
            }

            nodesByKey.Clear();
            nodes.Clear();
        }

        public V Get(K key)
        {
            if (nodesByKey.TryGetValue(key, out var node))
            {
                nodes.Remove(node);
                nodes.AddLast(node);
            }
            else
            {
                var value = valueConstructor(key);
                node = nodes.AddLast((key, value));
                nodesByKey.Add(key, node);
            }

            TrimSize();

            return node.Value.Item2;
        }

        private void TrimSize()
        {
            while (nodesByKey.Count > size)
            {
                var node = nodes.First;
                evictionCallback(node.Value.Item2);
                nodes.Remove(node);
                nodesByKey.Remove(node.Value.Item1);
            }
        }
    }
}