using System;
using System.Collections;
using System.Collections.Generic;

namespace NWindows.Examples.Controls
{
    public interface IReadOnlyLinkedHashSet<T> : IReadOnlyCollection<T>
    {
        T First { get; }
        T Last { get; }
        bool TryGetNextValue(T value, out T nextValue);
        bool TryGetPreviousValue(T value, out T prevValue);
    }

    public class LinkedHashSet<T> : IReadOnlyLinkedHashSet<T>
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

        public T First
        {
            get
            {
                var firstNode = nodes.First;
                if (firstNode == null)
                {
                    throw new InvalidOperationException($"Cannot get first element of an empty {nameof(LinkedHashSet<T>)}.");
                }

                return firstNode.Value;
            }
        }

        public T Last
        {
            get
            {
                var lastNode = nodes.Last;
                if (lastNode == null)
                {
                    throw new InvalidOperationException($"Cannot get last element of an empty {nameof(LinkedHashSet<T>)}.");
                }

                return lastNode.Value;
            }
        }

        public bool TryGetNextValue(T value, out T nextValue)
        {
            if (!nodesByValue.TryGetValue(value, out var node))
            {
                throw new InvalidOperationException($"{nameof(LinkedHashSet<T>)} does not have the passed value.");
            }

            var nextNode = node.Next;
            if (nextNode == null)
            {
                nextValue = default(T);
                return false;
            }

            nextValue = nextNode.Value;
            return true;
        }

        public bool TryGetPreviousValue(T value, out T prevValue)
        {
            if (!nodesByValue.TryGetValue(value, out var node))
            {
                throw new InvalidOperationException($"{nameof(LinkedHashSet<T>)} does not have the passed value.");
            }

            var prevNode = node.Previous;
            if (prevNode == null)
            {
                prevValue = default(T);
                return false;
            }

            prevValue = prevNode.Value;
            return true;
        }
    }
}