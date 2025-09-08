package wumpusworld.behaviors;
import java.util.function.Consumer;
import wumpusworld.Behavior;

public class Sequence extends Behavior
{
    public Sequence(String description) { super(description); }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        for (int i = 0; i < getChildCount(); i++)
            if (!getChild(i).run(dataFunction, context))
                return false;

        dataFunction.accept(this);
        return true;
    }
}
