package wumpusworld.behaviors;
import java.util.function.Consumer;
import wumpusworld.Behavior;
import wumpusworld.Agent;

public class DebugKnowledge extends Behavior
{
    public DebugKnowledge(String description) { super(description); }
    public boolean isLeaf() { return true; }

    public boolean run(Consumer<Behavior> dataFunction, Object context)
    {
        Agent agent = (Agent) context;
        Agent.Knowledge knowledge = agent.getKnowledge();

        System.out.println("Agent World Model (With Stimuli)");

        for (int xIndex = 0; xIndex < knowledge.modelWorld.length; xIndex++)
            System.out.print("------");
        System.out.println("-");

        for (int yIndex = 0; yIndex < knowledge.modelWorld[0].length; yIndex++)
        {
            for (int xIndex = 0; xIndex < knowledge.modelWorld.length; xIndex++)
                System.out.print("| " + knowledge.getStateAsString(xIndex, yIndex) + " ");
            System.out.println("|");

            for (int xIndex = 0; xIndex < knowledge.modelWorld.length; xIndex++)
                System.out.print("| " + knowledge.getStimuliAsString(xIndex, yIndex) + " ");
            System.out.println("|");

            if (yIndex < knowledge.modelWorld[0].length - 1)
            {
                for (int xIndex = 0; xIndex < knowledge.modelWorld.length; xIndex++)
                    System.out.print("|-----");
                System.out.println("|");
            }
            else
            {
                for (int xIndex = 0; xIndex < knowledge.modelWorld.length; xIndex++)
                    System.out.print("------");
                System.out.println("-\n");
            }
        }

        dataFunction.accept(this);
        return true;
    }
}
