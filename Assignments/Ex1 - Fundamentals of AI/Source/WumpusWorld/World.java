//! \file World.java
//! \brief Defines the <code>World</code> class.
//! \author Jeremiah Blanchard
package wumpusworld;

// friend class Game; <-- OK without this?...

public class World
{
    // Bit-flags for world room states
    public static final class Stimuli
    {
        public static final int NONE       = 0x00000000;
        public static final int STENCH     = 0x00000001;
        public static final int BREEZE     = 0x00000002;
        public static final int GOLD       = 0x00000004;
        public static final int UNEXPLORED = 0x00000008;
        public static final int PIT        = 0x00000010;
        public static final int WUMPUS     = 0x00000020;
        public static final int START      = 0x00000040;
//        private Stimuli() {}
    }

    private int[][] stimulus;
    private int width, height;

    public int agentX, agentY;
    public boolean agentAlive;
    public boolean wumpusAlive;
    public boolean goldRetrieved;
    public boolean agentHasArrow;

    // Constructor
    public World(int[][] _stimulus, int _width, int _height)
    {
        width = _width;
        height = _height;
        stimulus = new int[width][height];

        // Deep-copy stimulus grid
        for (int x = 0; x < width; x++)
        {
            System.arraycopy(_stimulus[x], 0, stimulus[x], 0, height);
            for (int y = 0; y < height; y++)
            {
                if ((stimulus[x][y] & Stimuli.START) != 0)
                {
                    agentX = x;
                    agentY = y;
                }
            }
        }

        agentAlive = true;
        wumpusAlive = true;
        goldRetrieved = false;
        agentHasArrow = true;
    }

    // Get methods
    public int getStimulus() { return stimulus[agentX][agentY]; } // Stimuli at agent location
    public int getHeight() { return height; }
    public int getWidth() { return width; }

    // Agent actions

    public boolean moveAgent(Agent.Direction direction)
    {
        boolean success = false;

        switch (direction)
        {
            case UP:
                if (agentY > 0) { agentY--; success = true; }
                break;
            case DOWN:
                if (agentY < height - 1) { agentY++; success = true; }
                break;
            case LEFT:
                if (agentX > 0) { agentX--; success = true; }
                break;
            case RIGHT:
                if (agentX < width - 1) { agentX++; success = true; }
                break;
        }

        // If we moved onto a WUMPUS or PIT, the agent dies.
        if ((stimulus[agentX][agentY] & (Stimuli.WUMPUS | Stimuli.PIT)) != 0)
            agentAlive = false;

        return success;
    }

    public boolean retrieveGold()
    {
        if ((stimulus[agentX][agentY] & Stimuli.GOLD) != 0)
        {
            stimulus[agentX][agentY] ^= Stimuli.GOLD;
            goldRetrieved = true;
            return true;
        }
        return false;
    }

    public void attackWumpus(Agent.Direction direction)
    {
        if (!agentHasArrow)
            return;

        agentHasArrow = false;

        switch (direction)
        {
            case UP:
                if (agentY > 0 && (stimulus[agentX][agentY - 1] & Stimuli.WUMPUS) != 0)
                {
                    stimulus[agentX][agentY - 1] ^= Stimuli.WUMPUS;
                    wumpusAlive = false;
                }
                return;
            case DOWN:
                if (agentY < height - 1 && (stimulus[agentX][agentY + 1] & Stimuli.WUMPUS) != 0)
                {
                    stimulus[agentX][agentY + 1] ^= Stimuli.WUMPUS;
                    wumpusAlive = false;
                }
                return;
            case LEFT:
                if (agentX > 0 && (stimulus[agentX - 1][agentY] & Stimuli.WUMPUS) != 0)
                {
                    stimulus[agentX - 1][agentY] ^= Stimuli.WUMPUS;
                    wumpusAlive = false;
                }
                return;

            case RIGHT:
                if (agentX < width - 1 && (stimulus[agentX + 1][agentY] & Stimuli.WUMPUS) != 0)
                {
                    stimulus[agentX + 1][agentY] ^= Stimuli.WUMPUS;
                    wumpusAlive = false;
                }
                return;
        }
    }
}
