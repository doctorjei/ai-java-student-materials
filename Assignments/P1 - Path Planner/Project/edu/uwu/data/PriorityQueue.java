//! \file PriorityQueue.java
//! \brief Defines the <code>PriorityQueue</code> class template.
//! \author Cromwell D. Enage, 2008
//! \author Jeremiah Blanchard, 2016, 2025
package edu.uwu.data;

import java.util.ArrayList;
import java.util.Comparator;

//! \brief The open heap used by all cost-based search algorithms.
//!
//! This class is basically a thin wrapper on top of ArrayList and provides
//! heap operations with a custom comparator.
public class PriorityQueue<T>
{
    final private ArrayList<T> list = new ArrayList<>();
    final private Comparator<T> comparator;

    public PriorityQueue(Comparator<T> _comparator) { comparator = _comparator; } // Constructor
    public boolean isEmpty() { return list.isEmpty(); } // Removes all nodes from the heap.
    public void clear() { list.clear(); } // Removes all nodes from the heap.
    public int size() { return list.size(); } // Returns the number of nodes currently in the heap.

    // Pushes the specified element onto heap, maintaining ordering during operation.
    public void push(T element)
    {
        // Find the insertion position using binary search
        int position = findPosition(element);
        list.add(position, element);
    }

    public T pop() { return list.remove(list.size() - 1); } // Removes highest priority element & returns it
    public void remove(T value) { list.remove(value); } // Remove first (should be only) instance

    // Remember: since we're pulling from the back of the queue, the sort order is "lowest last".
    int compare(T lhs, T rhs) { return -comparator.compare(lhs, rhs); }

    public ArrayList<T> enumerate() { return new ArrayList<>(list); }

    // Helper method to find the insertion position for a new node.
    // Uses binary search to find the upper bound position.
    private int findPosition(T element)
    {
        int low = 0;
        int high = list.size();

        while (low < high)
        {
            int mid = (low + high) / 2;

            if (compare(list.get(mid), element) >= 0)
                high = mid;
            else
                low = mid + 1;
        }
        return low;
    }
}
