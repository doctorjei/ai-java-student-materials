package ai_structures;

//import java.util.Iterator;
//import java.lang.Iterable;

public class LinkedList<T> implements Iterable<T>
{    
    // Private (protect it from programs trying to mess the doubly linked list connections) node class
    private class Node
    {
        protected T data;
        protected Node nextNode;
        protected Node prevNode;

        // Constructor
        public Node(T data) // We do not set next and prev here!
        {
            this.data = data;
            this.nextNode = null;
            this.prevNode = null;
        }
    }

    // Variables
    private Node frontNode;
    private Node backNode;

    // Iterator Class
    public class Iterator implements java.util.Iterator<T>
    {
        private Node currentNode;

        // lazy assignment?
        public Iterator()
        {
            this.currentNode = null;
        }

        public Iterator(Node firstNode) 
        {
            this.currentNode = firstNode;
        }

        public T next()
        {
            T tempDataHold = currentNode.data;
            currentNode = currentNode.nextNode;
            return tempDataHold;
        }

        public boolean hasNext()
        {
            return (currentNode != null);
        }

        public boolean equals(java.util.Iterator<T> rhs)
        {
            if (rhs == null)
            {
                return false;
            }

            return this.currentNode == ((Iterator) rhs).currentNode;
        }
    }

    // Constructor - should be empty at the start
    public LinkedList() // Not sure why this error happens when I do public LinkedList<T>(): "Syntax error on token ">", Identifier expected after this token"
    {
        this.frontNode = null;
        this.backNode = null;
    }

    // Returns an Iterator pointing to the beginning of the list.
    public Iterator iterator()
    {
        return new Iterator(this.frontNode);
    }

    public boolean isEmpty()
    {
        return ((this.frontNode == null) && (this.backNode == null));
    }

    public T getFront()
    {
        return this.frontNode.data;
    }

    public T getBack()
    {
        return this.backNode.data;
    }

    public void enqueue(T element) // Insert in end of list
    {
        if (isEmpty())
        {
            this.frontNode = new Node(element);
            this.backNode = this.frontNode;
            return;
        }
        
        this.backNode.nextNode = new Node(element);
        this.backNode.nextNode.prevNode = this.backNode;
        this.backNode = this.backNode.nextNode;
    }

    public void clear()
    {
        this.backNode = null;
        this.frontNode = null;
    }

    public void dequeue()
    {
        // Remove first element
        if (isEmpty())
        {
            return;
        }

        if (this.frontNode.equals(this.backNode))
        {
            clear();
            return;
        }

        this.frontNode = this.frontNode.nextNode;
        this.frontNode.prevNode = null;
    }

    public void pop()
    {
        // remove last element
        if (isEmpty())
        {
            return;
        }

        if (this.frontNode.equals(this.backNode))
        {
            clear();
            return;
        }

        this.backNode = this.backNode.prevNode;
        this.backNode.nextNode = null;
    }

    public boolean contains(T element)
    {
        //find a node whose data equals the specified element
        Iterator tempIter = iterator();

        while (tempIter.hasNext())
        {
            if (element.equals(tempIter.next()))
            {
                return true;
            }
        }
        return false;
    }

    public void remove(T element)
    {
        //Removes the first node you find whose data equals the specified element.
        if (contains(element))
        {
            Node tempCurrentNode = this.frontNode;

            while (tempCurrentNode != null)
            {
                if (tempCurrentNode.data.equals(element))
                {                    
                    // If it is the only element
                    if ((tempCurrentNode.prevNode == null) && (tempCurrentNode.nextNode == null))
                    {
                        clear();
                    }
                    // If it is in the start
                    else if (tempCurrentNode.prevNode == null)
                    {
                        dequeue();
                    }
                    // if it is in the last
                    else if (tempCurrentNode.nextNode == null)
                    {
                        pop();
                    }
                    // If it is in the middle
                    else 
                    {
                        tempCurrentNode.nextNode.prevNode = tempCurrentNode.prevNode;
                        tempCurrentNode.prevNode.nextNode = tempCurrentNode.nextNode;
                    }
                    break;
                }

                tempCurrentNode = tempCurrentNode.nextNode;
            }
        }
    }
}

