namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;

    public class PriorityQueue<T>
    {
        public int Count => list.Count;

        private readonly IComparer<T> comparer;
        private readonly List<T> list;

        public PriorityQueue(IComparer<T> comparer)
        {
            this.comparer = comparer;
            list = new List<T>();
        }

        public void Enqueue(T x)
        {
            list.Remove(x);
            list.Add(x);
            var i = Count - 1;

            while (i > 0)
            {
                var p = (i - 1) / 2;
                if (comparer.Compare(list[p], x) < 0)
                {
                    break;
                }

                list[i] = list[p];
                i = p;
            }

            if (Count > 0)
            {
                list[i] = x;
            }
        }

        public T Dequeue()
        {
            var min = Peek();
            var root = list[Count - 1];
            list.RemoveAt(Count - 1);

            var i = 0;
            while (i * 2 + 1 < Count)
            {
                var a = i * 2 + 1;
                var b = i * 2 + 2;
                var c = b < Count && comparer.Compare(list[b], list[a]) < 0 ? b : a;

                if (comparer.Compare(list[c], root) >= 0)
                {
                    break;
                }
                list[i] = list[c];
                i = c;
            }

            if (Count > 0) list[i] = root;
            return min;
        }

        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }
            return list[0];
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}
