//! \file Agent.java
//! \brief Defines the <code>Agent</code> class.
//! \author Jeremiah Blanchard
package wumpusworld;

import java.util.function.Consumer;
import wumpusworld.World;
import wumpusworld.Behavior;

//! \brief The Agent class for this project
public class Agent
{
    // Simple direction enum
    public enum Direction { UP, DOWN, LEFT, RIGHT }

    public static final class Knowledge
    {
        public static final class Status
        {
            // World-state information is tracked using status and  flags for each location.
            public static final int CLEAR = 0;
            public static final int DEFINITE_WUMPUS = 1;
            public static final int DEFINITE_PIT = 2;
            public static final int POSSIBLE_WUMPUS = 3;
            public static final int POSSIBLE_PIT = 4;
            public static final int POSSIBLE_W_P = 5;
            public static final int UNKNOWN = -1;
        }

        public int[][] stimuli;     // stimulus flags the agent has perceived
        public int[][] modelWorld;  // agent’s internal model/labels

        public int x, y;              // Location of agent in world currently
        public int wumpusX, wumpusY;  // Location of the Wumpus (init to -1, -1)
        public boolean canExplore;    // Are safe, unexplored locations around this spot?
        public boolean hasArrow;      // Whether or not our agent has the arrow
        public boolean hasGold;       // Whether or not our agent has the gold

	// Initialize Knowledge
        public void init(int _x, int _y, int width, int height)
        {
            // Erase our knowledge of the world.
            modelWorld = new int[width][height];
            stimuli = new int[width][height];

            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                for (int yIndex = 0; yIndex < height; yIndex++)
                {
                    modelWorld[xIndex][yIndex] = Status.UNKNOWN;
                    stimuli[xIndex][yIndex] = World.Stimuli.UNEXPLORED;
                }
            }

            // Forget the previous wumpus location, reset position, & reset game state.
            wumpusX = wumpusY = -1;
            hasGold = false;
            hasArrow = true;
            x = _x;
            y = _y;
        }

	// Clear out Agent Knowledge
        public void shutdown()
        {
            // modelWorld.clear();
            modelWorld = null;
            stimuli = null;
        }

        // Render model state at (_x,_y)
        public String getStateAsString(int _x, int _y)
        {
            if (x == _x && y == _y)
                return " ☺ ";

            switch (modelWorld[_x][_y])
            {
                case Status.UNKNOWN:          return "???";
                case Status.CLEAR:            return "   ";
                case Status.DEFINITE_WUMPUS:  return " Ω ";
                case Status.DEFINITE_PIT:     return " ○ ";
                case Status.POSSIBLE_WUMPUS:  return "?Ω?";
                case Status.POSSIBLE_PIT:     return "?○?";
                case Status.POSSIBLE_W_P:     return "Ω○?";
                default:                      return "WAT";
            }
        }

        public String getStimuliAsString(int _x, int _y)
        {
            // Index is from the low 3 bits (same as C++: STENCH=1, BREEZE=2, GOLD=4)
            final String[] stimStrings =
            {
                "   ",    // 0
                "‼  ",    // 1 = STENCH
                " ~ ",    // 2 = BREEZE
                "‼~ ",    // 3 = STENCH|BREEZE
                "  ☼",    // 4 = GOLD
                "‼ ☼",    // 5 = STENCH|GOLD
                " ~☼",    // 6 = BREEZE|GOLD
                "‼~☼"     // 7 = STENCH|BREEZE|GOLD
            };
            int index = stimuli[_x][_y] & (World.Stimuli.STENCH | World.Stimuli.BREEZE | World.Stimuli.GOLD);
            return stimStrings[index];
        }
    }

    private final World world; // The outside world
    private final Behavior behavior; // Agent behavior
    private final Knowledge knowledge; // Knowledge agent has about world
    private final Consumer<Behavior> behaviorLog; // behavior logging lambda

    // Instantiate an agent.
    public Agent(World _world, Behavior _behavior, Consumer<Behavior> _behaviorLog)
    {
        world = _world;
        behavior = _behavior;
        behaviorLog = _behaviorLog;
        knowledge = new Knowledge();
    }

    public Knowledge getKnowledge() { return knowledge; } // Returns reference to agent knowledge

    // Begin agent functionality; erase our knowledge of the world.
    public void enter(int x, int y) { knowledge.init(x, y, world.getWidth(), world.getHeight()); }
    public void update() { perceive(); behavior.run(behaviorLog, this); } // Update agent behavior
    public void exit() { knowledge.shutdown(); } // Shut down the agent.

    // Agent actions
    public boolean pickUpGold()
    {
        if (world.retrieveGold())
        {
            knowledge.hasGold = true;
            return true;
        }
        return false;
    }

    public boolean move(Direction direction)
    {
        if (world.moveAgent(direction))
        {
            switch (direction)
            {
                case UP:    knowledge.y--; break;
                case DOWN:  knowledge.y++; break;
                case LEFT:  knowledge.x--; break;
                case RIGHT: knowledge.x++; break;
            }
            return true;
        }
        return false;
    }

    public boolean shoot(Direction direction)
    {
        if (knowledge.hasArrow)
        {
            world.attackWumpus(direction);
            knowledge.hasArrow = false;
            return true;
        }
        return false;
    }

    // Gather stimulus from the world state at current (x,y).
    private void perceive() { knowledge.stimuli[knowledge.x][knowledge.y] = world.getStimulus(); }
}
