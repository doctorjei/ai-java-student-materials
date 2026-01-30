package ai_structures;

import java.util.ArrayList;
import java.util.function.Consumer; // Java 8, this prevents the errors
//import java.util.Queue;

public class TreeNode<T>
{
    // Variables
    private T parentData;
    private ArrayList<TreeNode<T>> childrenOfParent;

    // No Arg Constructor
    public TreeNode()
    {
        // Constructor for TreeNode. Should store null as its data value and start with no children.
        this.parentData = null;
        this.childrenOfParent = new ArrayList<TreeNode<T>>();
    }

    // Constructor
    public TreeNode(T element)
    {
        // Constructor for TreeNode. Should store element as its data value and start with no children.
        this.parentData = element;
        this.childrenOfParent = new ArrayList<TreeNode<T>>();
    }

    public T getData()
    {
        return this.parentData; // Returns a reference to the stored data.
    }

    public int getChildCount()
    {
        return this.childrenOfParent.size(); // Returns the number of children of this node.
    }

    public TreeNode<T> getChild(int index)
    {
        return this.childrenOfParent.get(index); // Returns the child node as specified by index.
    }

    public void addChild(TreeNode<T> child)
    {
        // Add child to the children of this node.
        this.childrenOfParent.add(child); // NOTE TO SELF: Do I have to sort it like a bin search tree?
    }

    public TreeNode<T> removeChild(int index)
    {
        return this.childrenOfParent.remove(index); //Remove and return the child node at specified by index. (Note that this does not delete the node!)
    }

    public void breadthFirstTraverse(Consumer<T> dataFunction)
    {
        // Breadth-first traversal starting at this node. Calls dataFunction.accept() on element to process it.
        ArrayList<TreeNode<T>> queue = new ArrayList<TreeNode<T>>();
        TreeNode<T> node;
        queue.add(this);

        while (!queue.isEmpty())
        {
            node = queue.removeFirst();
            dataFunction.accept(node.parentData);

            for (int i = 0; i < node.childrenOfParent.size(); i++)
            {
                queue.addLast(node.getChild(i));
            }
        }
    }

    public void preOrderTraverse(Consumer<T> dataFunction)
    {
        // Pre-order traversal starting at this node. Calls dataFunction.accept() on the element to process it.
        dataFunction.accept(this.parentData);

        for (int i = 0; i < this.childrenOfParent.size(); i++)
        {
            this.getChild(i).preOrderTraverse(dataFunction);
        }
    }

    public void postOrderTraverse(Consumer<T> dataFunction)
    {
        // Post-order traversal starting at this node. Calls dataFunction.accept() on the element to process it.
        for (int i = 0; i < this.childrenOfParent.size(); i++)
        {
            this.getChild(i).postOrderTraverse(dataFunction);
        }

        dataFunction.accept(this.parentData);
    }
}
