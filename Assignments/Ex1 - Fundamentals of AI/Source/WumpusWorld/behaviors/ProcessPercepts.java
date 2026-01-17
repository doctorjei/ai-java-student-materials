package wumpusworld.behaviors;

import wumpusworld.Agent;
import wumpusworld.Agent.Knowledge;
import wumpusworld.Behavior;

import static wumpusworld.Agent.Knowledge.Status.*;
import wumpusworld.Agent.Direction;
import wumpusworld.World.Stimuli;

public class ProcessPercepts extends Behavior
{
    public ProcessPercepts(String description) { super(description); }
    public boolean isLeaf() { return true; }

    public boolean run(java.util.function.Consumer<Behavior> dataFunction, Object context)
    {
        // Offsets for looking around a square.
        int offset[][] = { {-1, 0}, {1, 0}, {0, -1}, {0, 1} };

        Agent agent = (Agent) context;
        Knowledge knowledge = agent.getKnowledge();
        int x = knowledge.x, y = knowledge.y;
        int[][] stimuli = knowledge.stimuli;
        int[][] modelWorld = knowledge.modelWorld;

        boolean breeze = (stimuli[x][y] & Stimuli.BREEZE) != 0;
        boolean stench = (stimuli[x][y] & Stimuli.STENCH) != 0;

        // If there's no breeze or stench, then boxes adjacent to this square are clear.
        if (!breeze && !stench)
        {
            for (int index = 0; index < 4; index++)
            {
                int newX = x + offset[index][0], newY = y + offset[index][1];

                if (newX >= 0 && newX < modelWorld.length && newY >= 0 && newY < modelWorld[newX].length)
                    modelWorld[newX][newY] = Knowledge.Status.CLEAR;
            }
        }

        // If there is a breeze here, there is a pit nearby. Update the agent's knowledge appropriately.
        if (breeze)
        {
            //Keep track of the non-pit spaces around this spot.
            int nonPitSpaces = 0;

            // First, count the non-pit spaces around the square.
            for (int index = 0; index < 4; index++)
            {
                int newX = x + offset[index][0];
                int newY = y + offset[index][1];

                if (newX >= 0 && newX < modelWorld.length && newY >= 0 && newY < modelWorld[newX].length)
                {
                    // If the space cannot hold a pit, add it to the non-pit spaces.
                    if (modelWorld[newX][newY] == Knowledge.Status.CLEAR || modelWorld[newX][newY] == Knowledge.Status.DEFINITE_WUMPUS)
                          nonPitSpaces++;
                }
                else
                    nonPitSpaces++; // If this is off of the map, it is not a pit space.
            }

            // Next, classify the space as best we can.
            for (int index = 0; index < 4; index++)
            {
                int newX = x + offset[index][0];
                int newY = y + offset[index][1];

                if (newX >= 0 && newX < modelWorld.length && newY >= 0 && newY < modelWorld[newX].length)
                {
                    // If there is only one possible pit and this is it,  mark it as such.
                    if (nonPitSpaces == 3 && modelWorld[newX][newY] != Knowledge.Status.CLEAR &&
                      modelWorld[newX][newY] != Knowledge.Status.DEFINITE_WUMPUS)
                        modelWorld[newX][newY] = Knowledge.Status.DEFINITE_PIT;

                    // If we believe that the space could hold a wumpus, mark it as possible wumpus OR pit.
                    else if (modelWorld[newX][newY] == Knowledge.Status.POSSIBLE_WUMPUS)
                        modelWorld[newX][newY] = Knowledge.Status.POSSIBLE_W_P;

                    // If we know nothing about the space, note that it is possibly a pit. (All other cases are covered.)
                    else if (modelWorld[newX][newY] == Knowledge.Status.UNKNOWN)
                        modelWorld[newX][newY] = Knowledge.Status.POSSIBLE_PIT;
                }
            }
        }

        // If there is a stench and we have not yet fixed the location of the wumpus, we should do so now.
        if (stench && (knowledge.wumpusX == -1 || knowledge.wumpusY == -1))
        {
            //Keep track of the non-pit spaces around this spot.
            int nonWumpusSpaces = 0;

            // First, count the non-wumpus spaces around the square.
            for (int index = 0; index < 4; index++)
            {
                int newX = x + offset[index][0];
                int newY = y + offset[index][1];

                if (newX >= 0 && newX < modelWorld.length && newY >= 0 && newY < modelWorld[newX].length)
                {
                    // If the space cannot hold the wumpus, add it to the non-wumpus spaces.
                    if (modelWorld[newX][newY] == Knowledge.Status.CLEAR || modelWorld[newX][newY] == Knowledge.Status.DEFINITE_PIT)
                        nonWumpusSpaces++;
                }
                else
                    nonWumpusSpaces++; // If this is off of the map, it is not the wumpus space.
            }

            // Next, classify the space as best we can.
            for (int index = 0; index < 4; index++)
            {
                int newX = x + offset[index][0];
                int newY = y + offset[index][1];

                if (newX >= 0 && newX < modelWorld.length && newY >= 0 && newY < modelWorld[newX].length)
                    // If there is only one possible wumpus space and this is it, mark it as such.
                    if (nonWumpusSpaces == 3 && modelWorld[newX][newY] != Knowledge.Status.CLEAR && modelWorld[newX][newY] != Knowledge.Status.DEFINITE_PIT)
                    {
                        modelWorld[newX][newY] = Knowledge.Status.DEFINITE_WUMPUS;
                        knowledge.wumpusX = newX;
                        knowledge.wumpusY = newY;

                        // Once we have found the wumpus, we can remove any
                        // other "wumpus" marks from our knowledge of the world.
                        for (int xIndex = 0; xIndex < modelWorld.length; xIndex++)
                            for (int yIndex = 0; yIndex < modelWorld[xIndex].length; yIndex++)
                            {
                                if (modelWorld[xIndex][yIndex] == Knowledge.Status.POSSIBLE_WUMPUS)
                                    modelWorld[xIndex][yIndex] = Knowledge.Status.UNKNOWN;

                                else if (modelWorld[xIndex][yIndex] == Knowledge.Status.POSSIBLE_W_P)
                                    modelWorld[xIndex][yIndex] = Knowledge.Status.POSSIBLE_PIT;
                            }
                    }
                    // If we believe that the space could hold a pit, mark it as possible pit OR wumpus.
                    else if (modelWorld[newX][newY] == Knowledge.Status.POSSIBLE_PIT)
                        modelWorld[newX][newY] = Knowledge.Status.POSSIBLE_W_P;

                    // If we know nothing about the space, note that it is possibly the wumpus. (All othe>
                    else if (modelWorld[newX][newY] == Knowledge.Status.UNKNOWN)
                        modelWorld[newX][newY] = Knowledge.Status.POSSIBLE_WUMPUS;
            }
        }

        // From here, are there any safe, unexplored locations, or do we need to back track?
        knowledge.canExplore = false;

        for (int index = 0; index < 4; index++)
        {
            int newX = x + offset[index][0];
            int newY = y + offset[index][1];

            if (newX >= 0 && newX < modelWorld.length && newY >= 0 && newY < modelWorld[newX].length)
            {
                if ((stimuli[newX][newY] & Stimuli.UNEXPLORED) != 0 && (modelWorld[newX][newY] == Knowledge.Status.CLEAR))
                {
                    knowledge.canExplore = true;
                    break;
                }
            }
        }

        dataFunction.accept(this);
        return true;
    }
}
