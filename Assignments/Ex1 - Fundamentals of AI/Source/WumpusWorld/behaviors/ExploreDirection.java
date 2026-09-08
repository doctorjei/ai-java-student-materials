package WumpusWorld.behaviors;
import java.util.function.Consumer;

import WumpusWorld.Agent;
import WumpusWorld.Agent.Direction;
import WumpusWorld.Agent.Knowledge;
import WumpusWorld.Behavior;
import WumpusWorld.World.Stimuli;

public class ExploreDirection extends Behavior
{
    private final Direction direction;
    public boolean isLeaf() { return true; }

    public ExploreDirection(String description, Direction direction)
    {
        super(description);
        this.direction = direction;
    }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        Agent agent = (Agent) context;
        Knowledge knowledge = agent.getKnowledge();
        int x = knowledge.x, y = knowledge.y;

        if (knowledge.canExplore)
        {
            switch (direction)
            {
                case UP:
                    if (y <= 0 || knowledge.modelWorld[x][y - 1] != Knowledge.Status.CLEAR
                            || (knowledge.stimuli[x][y - 1] & Stimuli.UNEXPLORED) == 0) return false;
                    break;
                case DOWN:
                    if (y >= knowledge.modelWorld[0].length - 1 || knowledge.modelWorld[x][y + 1] != Knowledge.Status.CLEAR
                            || (knowledge.stimuli[x][y + 1] & Stimuli.UNEXPLORED) == 0) return false;
                    break;
                case LEFT:
                    if (x <= 0 || knowledge.modelWorld[x - 1][y] != Knowledge.Status.CLEAR
                            || (knowledge.stimuli[x - 1][y] & Stimuli.UNEXPLORED) == 0) return false;
                    break;
                case RIGHT:
                    if (x >= knowledge.modelWorld.length - 1 || knowledge.modelWorld[x + 1][y] != Knowledge.Status.CLEAR
                            || (knowledge.stimuli[x + 1][y] & Stimuli.UNEXPLORED) == 0) return false;
                    break;
            }
        }

        if (agent.move(direction))
        {
            dataFunction.accept(this);
            return true;
        }
        return false;
    }
}
