//! \file Game.java
//! \brief Entry point of Wumpus World game; fefines the <code>Game</code> class.
//! \author Jeremiah Blanchard, August 2009
// Updated by Jeremiah Blanchard, January 2012, September 2025
package wumpusworld;

import java.util.function.Consumer;
import java.util.function.Supplier;

import wumpusworld.Behavior;
import wumpusworld.behaviors.*;
import static wumpusworld.World.Stimuli.*;
import static wumpusworld.Agent.Direction;

public final class Game
{
    private static final int[][] WORLD1 =
    {
        { NONE, STENCH, WUMPUS, STENCH, NONE, NONE },
        { NONE, NONE, (BREEZE | STENCH | GOLD), NONE, NONE, NONE },
        { NONE, BREEZE, PIT, BREEZE, NONE, BREEZE },
        { NONE, NONE, BREEZE, BREEZE, BREEZE, PIT },
        { NONE, NONE, BREEZE, PIT, BREEZE, BREEZE },
        { START, NONE, NONE, BREEZE, NONE, NONE }
    };

    public static void run()
    {
        // Set up the logging.
        StringBuilder behaviorLog = new StringBuilder();

        Consumer<Behavior> logBehavior = behavior ->
        {
            if (behavior.isLeaf())
                behaviorLog.append(behavior.toString()).append('\n');
        };

        Supplier<String> logFetcher = () ->
        {
            String log = behaviorLog.toString();
            behaviorLog.setLength(0);
            return log;
        };

        // Copy WORLD1 into a new 2D array.
        int sizeX = WORLD1.length, sizeY = WORLD1[0].length;
        int[][] worldData = new int[sizeX][sizeY];

        for (int x = 0; x < sizeX; x++)
            System.arraycopy(WORLD1[x], 0, worldData[x], 0, sizeY);

        wumpusworld.World world = new wumpusworld.World(worldData, sizeX, sizeY);
        Behavior behavior = buildTree();

        wumpusworld.Agent agent = new wumpusworld.Agent(world, behavior, logBehavior);
        agent.enter(world.agentX, world.agentY);

        System.out.println("\nWorld Information\n-----------------");
        System.out.println("Agent Position: (" + world.agentX + ", " + world.agentY + ")");
        System.out.println("Agent Has Arrow: " + world.agentHasArrow);
        System.out.println("Agent Has Gold: " + world.goldRetrieved);
        System.out.println("Agent is Alive: " + world.agentAlive);
        System.out.println("Wumpus is Alive: " + world.wumpusAlive);
        System.out.println();

        while (world.agentAlive && world.agentHasArrow)
        {
            agent.update();
            System.out.println("Leaf Behaviors\n--------------");
            System.out.print(logFetcher.get());

            System.out.println("\nWorld Information\n-----------------");
            System.out.println("Agent Position: (" + world.agentX + ", " + world.agentY + ")");
            System.out.println("Agent Has Arrow: " + world.agentHasArrow);
            System.out.println("Agent Has Gold: " + world.goldRetrieved);
            System.out.println("Agent is Alive: " + world.agentAlive);
            System.out.println("Wumpus is Alive: " + world.wumpusAlive);
            System.out.println();
        }

        if (!world.agentAlive)
            System.out.println("You died!");

        if (world.goldRetrieved)
            System.out.println("You found the gold!");

        if (!world.wumpusAlive)
            System.out.println("You killed the wumpus!");

        disconnectTree(behavior);
    }

    public static void main(String[] args)
    {
        Consumer<String> printString = testString -> System.out.println(testString);

        // First, run a general test of the behavior tree mechanisms.
        Behavior root = buildTree();
        System.out.println("\nBreadth-First:\n--------------");
        root.breadthFirstTraverse(printString);
        System.out.println("\nPreorder\n--------");
        root.preOrderTraverse(printString);
        System.out.println("\nPostorder\n---------");
        root.postOrderTraverse(printString);
        disconnectTree(root);

        System.out.println("Press ENTER to continue...");
        java.util.Scanner scanner = new java.util.Scanner(System.in);
        scanner.nextLine();

        // Then, run the wumpus world game simulation.
        Game.run();
        System.out.println("Press ENTER to continue...");
        scanner.nextLine();
    }

    private static Behavior buildTree()
    {
        Behavior behavior = new Sequence("Basic Behavior");
        behavior.addChild(new ProcessPercepts("Process Percepts"));
        behavior.addChild(new DebugKnowledge("Debug Knowledge"));
        behavior.addChild(new Selector("Choose Action"));
        behavior.getChild(2).addChild(new Sequence("Look For Gold"));
        behavior.getChild(2).getChild(0).addChild(new CheckForGold("Check For Gold"));
        behavior.getChild(2).getChild(0).addChild(new PickUpGold("Pick Up Gold"));
        behavior.getChild(2).addChild(new ShootWumpus("Shoot Wumpus"));
        behavior.getChild(2).addChild(new Selector("Explore"));
        behavior.getChild(2).getChild(2).addChild(new ExploreDirection("Explore Up", Direction.UP));
        behavior.getChild(2).getChild(2).addChild(new ExploreDirection("Explore Down", Direction.DOWN));
        behavior.getChild(2).getChild(2).addChild(new ExploreDirection("Explore Left", Direction.LEFT));
        behavior.getChild(2).getChild(2).addChild(new ExploreDirection("Explore Right", Direction.RIGHT));
        return behavior;
    }

    private static void disconnectTree(Behavior root)
    {
        java.util.LinkedList<Behavior> q = new java.util.LinkedList<>();
        q.add(root);

        while (!q.isEmpty())
        {
            Behavior current = q.pop();

            // Break the connections between nodes.
            for (int i = current.getChildCount() - 1; i >= 0; i--)
                q.add(current.removeChild(i));
        }
    }
}
