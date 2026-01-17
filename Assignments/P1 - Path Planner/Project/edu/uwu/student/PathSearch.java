package edu.uwu.student;

import edu.uwu.data.PriorityQueue;
import edu.uwu.tiles.Tile;
import edu.uwu.tiles.TileMap;
import java.util.ArrayList;
import java.util.HashMap;

public class PathSearch
{
    private static class SearchNode // Inner class: SearchNode
    {
        Tile tile;
        ArrayList<Edge> edges;

        SearchNode(Tile _tile)
        {
            tile = _tile;
            edges = new ArrayList<>();
        }
    }

    private static class Edge // Inner class: Edge
    {
        double cost;
        SearchNode endpoint;

        Edge(SearchNode _endpoint, double _cost)
        {
            endpoint = _endpoint;
            cost = _cost;
        }
    }

    private static class PlannerNode // Inner class: PlannerNode
    {
        PlannerNode parent;
        SearchNode vertex;
        double heuristicCost = 0, givenCost = 0, finalCost = 0;

        PlannerNode(SearchNode _vertex, PlannerNode _parent)
        {
            vertex = _vertex;
            parent = _parent;
        }

        PlannerNode(SearchNode _vertex)
        {
            this(_vertex, null);
        }
    }

    // Member variables
    private boolean doneSearching = false;
    private TileMap tileMap = null;
    private SearchNode goal = null;
    private double heuristicWeight;

    private long lastTick = 0;
    private int iterationCount = 0;
    private PlannerNode current = null;

    private HashMap<Tile, SearchNode> created;
    private PriorityQueue<PlannerNode> open;
    private HashMap<SearchNode, PlannerNode> visited;
    private ArrayList<Tile> solution;

    // Static helper methods
    private static boolean tilesAreAdjacent(Tile lhs, Tile rhs)
    {
        int row1 = lhs.getRow();
        int column1 = lhs.getColumn();
        int row2 = rhs.getRow();
        int column2 = rhs.getColumn();

        // If they are in the same row or column, see if they are offset by one.
        if (row1 == row2)
            return (column1 + 1 == column2) || (column2 + 1 == column1);
        else if (column1 == column2)
            return (row1 + 1 == row2) || (row2 + 1 == row1);
        // Otherwise, check the diagonals.
        else
            return ((row1 & 1) != 0 ? (column1 + 1 == column2) : (column2 + 1 == column1))
                && ((row1 + 1 == row2) || (row2 + 1 == row1));
    }

    private static double computeTileDistance(Tile lhs, Tile rhs)
    {
        double dx = lhs.getX() - rhs.getX();
        double dy = lhs.getY() - rhs.getY();
        return Math.sqrt(dx * dx + dy * dy);
    }

    private static int compare(PlannerNode lhs, PlannerNode rhs)
    {
        if (lhs.finalCost < rhs.finalCost)
            return -1;
        else if (lhs.finalCost > rhs.finalCost)
            return 1;
        else
            return 0;
    }

    // Constructor
    public PathSearch()
    {
        open = new PriorityQueue<>((lhs, rhs) -> compare(lhs, rhs));
        visited = new HashMap<>();
        created = new HashMap<>();
        solution = new ArrayList<>();
        heuristicWeight = 1.2;
    }

    // Public methods
    public void load(TileMap _tileMap)
    {
        int[][] offsets = {
            {-1, -1},
            {-1, 0},
            {-1, 1},
            {0, -1}
        };

        tileMap = _tileMap;

        // For each valid tile in the tile map...
        for (int row = 0; row < tileMap.getRowCount(); row++)
        {
            for (int column = 0; column < tileMap.getColumnCount(); column++)
            {
                Tile tile = tileMap.getTile(row, column);

                // If this tile is an obstacle, skip it.
                if (tile.getWeight() == 0)
                    continue;

                // Create a new node for the tile and determine its neighbors.
                SearchNode node = new SearchNode(tile);
                created.put(tile, node);

                for (int offset = 0; offset < 4; offset++)
                {
                    Tile neighborTile = tileMap.getTile(row + offsets[offset][0], column + offsets[offset][1]);
                    SearchNode neighbor = neighborTile == null ? null : created.get(neighborTile);

                    // If the neighboring node does not exist or is not adjacent, skip this iteration.
                    if (neighbor == null || !tilesAreAdjacent(tile, neighbor.tile))
                        continue;

                    // Create edges connecting the two nodes.
                    neighbor.edges.add(new Edge(node, tileMap.getTileRadius() * 2.0 * tile.getWeight()));
                    node.edges.add(new Edge(neighbor, tileMap.getTileRadius() * 2.0 * neighbor.tile.getWeight()));
                    tile.addLineTo(neighbor.tile, 0xFF000000 | (0x0000FFFF * (4 - offset) / 8));
                }
            }
        }
    }

    public void initialize(int startRow, int startColumn, int goalRow, int goalColumn)
    {
        SearchNode start = created.get(tileMap.getTile(startRow, startColumn));
        goal = created.get(tileMap.getTile(goalRow, goalColumn));
        current = new PlannerNode(start);

        current.finalCost = current.heuristicCost = computeTileDistance(current.vertex.tile, goal.tile) * heuristicWeight;
        visited.put(start, current);
        open.push(current);
        iterationCount = 0;
        doneSearching = false;
    }

    public void update(long timeslice)
    {
        lastTick = System.currentTimeMillis();

        while (!open.isEmpty())
        {
            current = open.pop();

            if (current.vertex == goal)
            {
                // Watch out for students trying to modify the current node.
                for (PlannerNode node = current; node != null; node = node.parent)
                    solution.add(node.vertex.tile);

                doneSearching = true;
                break;
            }

            for (Edge edge : current.vertex.edges)
            {
                double newGivenCost = current.givenCost + edge.cost;
                SearchNode successor = edge.endpoint;

                if (visited.containsKey(successor))
                {
                    PlannerNode successorNode = visited.get(successor);
                    if (newGivenCost < successorNode.givenCost)
                    {
                        successorNode.parent = current;
                        successorNode.givenCost = newGivenCost;
                        successorNode.finalCost = newGivenCost + successorNode.heuristicCost * heuristicWeight;
                        open.remove(successorNode);
                        open.push(successorNode);
                    }
                }
                else
                {
                    PlannerNode successorNode = new PlannerNode(successor, current);
                    successorNode.givenCost = newGivenCost;
                    successorNode.heuristicCost = computeTileDistance(successor.tile, goal.tile);
                    successorNode.finalCost = newGivenCost + successorNode.heuristicCost * heuristicWeight;

                    open.push(successorNode);
                    visited.put(successor, successorNode);
                }
            }

            if ((System.currentTimeMillis() - lastTick) >= timeslice)
                break;
        }

        // Set some drawing up to display debugging information.
        tileMap.resetTileDrawing();

        // Created (visited nodes)
        for (SearchNode node : visited.keySet())
            node.tile.setFill(0xFF0000FF);

        // Open
        var tmp = open.enumerate();

        // Change luminosity based on distance from front of queue.
        for (int index = 0; index < tmp.size(); index++)
        {
            int luminosity = (tmp.size() - index - 1) > 24 ? 64 : (((33 + index - tmp.size()) * 0xFF + 1) / 32);
            tmp.get(index).vertex.tile.setMarker((0xFF000000) | (luminosity << 8));
        }

        // Successors (according to adjacency function)
        for (Edge edge : current.vertex.edges)
        {
            edge.endpoint.tile.setOutline(0xFFFFC000);
            current.vertex.tile.addLineTo(edge.endpoint.tile, 0xFFFF8000);
        }

        // Path
        for (PlannerNode path = current; path != null; path = path.parent)
            if (path.parent != null)
                path.vertex.tile.addLineTo(path.parent.vertex.tile, 0xFFFF0000);
    }

    public void shutdown()
    {
        current = null;
        goal = null;
        solution.clear();
        open.clear();
        visited.clear();
    }

    public void unload()
    {
        created.clear();
    }

    public boolean isDone()
    {
        return doneSearching;
    }

    public ArrayList<Tile> getSolution()
    {
        return new ArrayList<>(solution); // Return defensive copy
    }
}
