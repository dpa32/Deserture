using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    public int Count => _heap.Count;

    private List<T> _heap = new List<T>();

    public void Enqueue(T item)
    {
        _heap.Add(item);
        int i = _heap.Count - 1;

        while (i > 0) // i != root
        {
            int parent = (i - 1) / 2;
            if (_heap[i].CompareTo(_heap[parent]) > 0)
            {
                break;
            }
            T temp = _heap[i];
            _heap[i] = _heap[parent];
            _heap[parent] = temp;

            i = parent;
        }
    }

    public T Dequeue()
    {
        if (_heap.Count <= 0)
        {
            return default(T);
        }

        T result = _heap[0];
        _heap[0] = _heap[_heap.Count - 1];
        _heap.RemoveAt(_heap.Count - 1);

        int i = 0;
        while (true)
        {
            int left = i * 2 + 1;
            int right = i * 2 + 2;
            int next = i;

            if (left < _heap.Count && _heap[left].CompareTo(_heap[next]) < 0)
            {
                next = left;
            }
            if (right < _heap.Count && _heap[right].CompareTo(_heap[next]) < 0)
            {
                next = right;
            }
            if(i == next)
            {
                break ;
            }
            T temp = _heap[i];
            _heap[i] = _heap[next];
            _heap[next] = temp;
            i = next;
        }
        return result;
    }

    public bool Contain(T t) => _heap.Contains(t);

}