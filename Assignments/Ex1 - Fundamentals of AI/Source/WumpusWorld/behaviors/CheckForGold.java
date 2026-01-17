package wumpusworld.behaviors;
import java.util.function.Consumer;
import wumpusworld.Behavior;
import wumpusworld.Agent;
import wumpusworld.World.Stimuli;

public class CheckForGold extends Behavior
{
    public CheckForGold(String description) { super(description); }
    public boolean isLeaf() { return true; }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        Agent agent = (Agent) context;
        Agent.Knowledge k = agent.getKnowledge();
        if ((k.stimuli[k.x][k.y] & Stimuli.GOLD) != 0)
        {
            dataFunction.accept(this);
            return true;
        }
        return false;
    }
}
