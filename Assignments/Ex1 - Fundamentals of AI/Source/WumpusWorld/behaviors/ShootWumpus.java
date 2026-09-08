package WumpusWorld.behaviors;
import java.util.function.Consumer;

import WumpusWorld.Agent;
import WumpusWorld.Agent.Direction;
import WumpusWorld.Behavior;
import WumpusWorld.World;

public class ShootWumpus extends Behavior
{
    public ShootWumpus(String description) { super(description); }
    public boolean isLeaf() { return true; }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        Agent agent = (Agent) context;
        Agent.Knowledge k = agent.getKnowledge();
        if ((k.stimuli[k.x][k.y] & World.Stimuli.STENCH) != 0 && agent.shoot(Direction.LEFT))
        {
            dataFunction.accept(this);
            return true;
        }
        return false;
    }
}
