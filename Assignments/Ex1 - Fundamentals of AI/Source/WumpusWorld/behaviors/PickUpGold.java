package wumpusworld.behaviors;
import java.util.function.Consumer;
import wumpusworld.Behavior;
import wumpusworld.Agent;

public class PickUpGold extends Behavior
{
    public PickUpGold(String description) { super(description); }
    public boolean isLeaf() { return true; }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        Agent agent = (Agent) context;
        if (agent.pickUpGold())
        {
            dataFunction.accept(this);
            return true;
        }
        return false;
    }
}
