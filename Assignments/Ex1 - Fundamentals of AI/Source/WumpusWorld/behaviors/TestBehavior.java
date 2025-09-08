package wumpusworld.behaviors;
import java.util.function.Consumer;
import wumpusworld.Behavior;

public class TestBehavior extends Behavior
{
    private final boolean value;
    public boolean isLeaf() { return true; }

    public TestBehavior(String description, boolean value)
    {
        super(description);
        this.value = value;
    }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        if (value)
        {
            dataFunction.accept(this);
            return true;
        }
        return false;
    }
}
