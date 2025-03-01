using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<T>
{
    private SortedDictionary<float, Queue<T>> elements = new SortedDictionary<float, Queue<T>>();

    public void Enqueue(T item, float priority)
    {
        if (!elements.ContainsKey(priority))
        {
            elements[priority] = new Queue<T>();
        }
        elements[priority].Enqueue(item);
    }

    public T Dequeue()
    {
        if (elements.Count == 0) return default;

        var firstKey = elements.Keys.Min();
        var queue = elements[firstKey];

        T item = queue.Dequeue();
        if (queue.Count == 0)
        {
            elements.Remove(firstKey);
        }

        return item;
    }

    public bool IsEmpty()
    {
        return elements.Count == 0;
    }

    public void Clear()
    {
        elements.Clear();
    }

    public bool Contains(T item)
    {
        // Iterate through all queues in the SortedDictionary
        foreach (var queue in elements.Values)
        {
            if (queue.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool Remove(T item)
    {
        // Iterate through all queues in the SortedDictionary
        foreach (var kvp in elements.ToList()) // Use ToList() to avoid modifying the collection while iterating
        {
            var queue = kvp.Value;
            if (queue.Contains(item))
            {
                // Create a new queue without the item
                var newQueue = new Queue<T>(queue.Where(x => !x.Equals(item)));

                if (newQueue.Count > 0)
                {
                    // Replace the old queue with the new queue
                    elements[kvp.Key] = newQueue;
                }
                else
                {
                    // If the queue is empty after removal, remove the priority entry
                    elements.Remove(kvp.Key);
                }

                return true; // Item removed
            }
        }
        return false; // Item not found
    }
}
