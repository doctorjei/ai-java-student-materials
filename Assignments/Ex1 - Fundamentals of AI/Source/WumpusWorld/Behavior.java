//! \file Behavior.java
//! \brief Defines the <code>Behavior</code> interface.
//! \author Jeremiah Blanchard
package wumpusworld;

import ai_structures.TreeNode;
import java.util.function.Consumer;

public abstract class Behavior extends TreeNode<String>
{
    // ================
    //   BASIC METHODS
    // ================
    public Behavior(String description) { super(description); } //! \brief New Behavior
    public boolean isLeaf() { return false; } //! \brief true if leaf node (Default: false)
    public String toString() { return super.getData(); } //! \brief String rep. of Behavior

    public int getChildCount() { return super.getChildCount(); }
    public String getData() { return super.getData(); }
    public Behavior getChild(int index) { return (Behavior) super.getChild(index); }
    public Behavior removeChild(int index) { return (Behavior) super.removeChild(index); }
    public void addChild(Behavior child) { super.addChild(child); }

    //=====================
    //  TRAVERSAL METHODS
    //=====================

    //! \brief Traverses the root and all sub-nodes breadth-first.
    public void breadthFirstTraverse(Consumer<String> dataFunction)
    {
        super.breadthFirstTraverse(dataFunction);
    }

    //! \brief Traverses the root and all sub-nodes in pre-order fashion.
    public void preOrderTraverse(Consumer<String> dataFunction)
    {
        super.preOrderTraverse(dataFunction);
    }

    //! \brief Traverses the root and all sub-nodes in post-order fashion.
    public void postOrderTraverse(Consumer<String> dataFunction)
    {
        super.postOrderTraverse(dataFunction);
    }

    //=======================================
    //  VIRTUAL (BEHAVIOR-SPECIFIC) METHODS
    //=======================================

    //! \brief Executes the behavior. Returns true (and runs dataFunction) on success, false otherwise.
    //!
    //! \param   dataFunction  a single-argument function that accepts the behavior as 
    //!                        a valid argument.
    //!
    //! \pre     <code>NULL !=</code> \a this
    public abstract boolean run(Consumer<Behavior> dataFunction, Object context);
}
