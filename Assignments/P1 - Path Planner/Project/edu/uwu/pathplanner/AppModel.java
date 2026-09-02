package edu.uwu.pathplanner;

import java.util.ArrayList;

import edu.uwu.tiles.Tile;
import edu.uwu.tiles.TileMap;
import edu.uwu.student.PathSearch;


public class AppModel
{
    public static final String hex006x006 = "./maps/hex006x006.txt";
    public static final String hex014x006 = "./maps/hex014x006.txt";
    public static final String hex035x035 = "./maps/hex035x035.txt";
    public static final String hex054x045 = "./maps/hex054x045.txt";
    public static final String hex098x098 = "./maps/hex098x098.txt";
    public static final String hex113x083 = "./maps/hex113x083.txt";

    public static final double MIN_RADIUS  = 8.0;
    public static final String DEFAULT_MAP = hex035x035; // change to swtich program default map
    // public static final String DEFAULT_MAP = hex098x098;

    private TileMap tileMap;
    private Tile startTile = null, goalTile = null;
    private String mapFile = DEFAULT_MAP;
    private PathSearch pathSearch;

    private boolean isLoaded = false, isInitialized = false;

    public AppModel()
    {
        pathSearch = new PathSearch();
        tileMap = new TileMap();
    }

    public boolean load() { return load(DEFAULT_MAP); }

    public boolean load(String _mapFile)
    {
        // Identify the correct file and load it.
        mapFile = ((_mapFile == null || _mapFile.isEmpty()) ? mapFile : _mapFile);
        mapFile = ((mapFile == null || mapFile.isEmpty()) ? DEFAULT_MAP : mapFile);

        try (java.io.InputStream fileIn = new java.io.FileInputStream(mapFile))
        {
            // If we can't read the info from the file, load fails; return false.
            if (!tileMap.loadFromStream(fileIn))
                return false;

            fileIn.close();
        }
        catch (Exception e)
        {
            return false;
        }

        // Gather info from the tile map and set its radius.
        int rowCount = tileMap.getRowCount();
        int colCount = tileMap.getColumnCount();
        double vRadius = Math.max(MIN_RADIUS, (rowCount < 20 ? 338.0 : 360.0) / rowCount);
        double hRadius = Math.max(MIN_RADIUS, (colCount < 20 ? 354.0 : 372.0) / colCount);
        tileMap.setRadius(Math.min(hRadius, vRadius)); // TODO: Remove magic numbers?

        // Load the path planner and move on to initialization.
        pathSearch.load(tileMap);
        isLoaded = true;
        return true;
    }

    public void initialize() { initialize(0, 0, 0, 0); }

    public void initialize(int startRow, int startColumn, int goalRow, int goalColumn)
    {
        if (!isLoaded)
            load();

        if (startRow == 0 && startColumn == 0 && goalRow == 0 && goalColumn == 0)
        {
            if (startTile != null)
            {
                startRow = startTile.getRow();
                startColumn = startTile.getColumn();
            }
            else
                startRow = startColumn = 0;

            if (goalTile != null)
            {
                goalRow = goalTile.getRow();
                goalColumn = goalTile.getColumn();
            }
            else
            {
                goalRow = tileMap.getRowCount() - 1;
                goalColumn = tileMap.getColumnCount() - 1;
            }
        }

        // Get the starting and goal location tiles to store internally.
        startTile = tileMap.getTile(startRow, startColumn);
        goalTile = tileMap.getTile(goalRow, goalColumn);

        // Wipe any path search in memory; then Clear out any drawing and initailize the search.
        pathSearch.shutdown();
        tileMap.resetTileDrawing();
        pathSearch.initialize(startRow, startColumn, goalRow, goalColumn);
        isInitialized = true;
    }

    public void update(long timeslice)
    {
        if (!isInitialized)
            initialize();

        pathSearch.update(timeslice);
    }

    public void shutdown()
    {
        pathSearch.shutdown();
        isInitialized = false;
    }

    public void unload()
    {
        if (isInitialized)
            shutdown();

        pathSearch.unload();
        isLoaded = false;
    }

    public String getMapFile() { return mapFile; }
    public TileMap getTileMap() { return tileMap; }
//public Tile getGoalTile() { return goalTile != null ? goalTile: goalTile = tileMap.getGoalTile(); }
//  required: variable
//   found:    value
    public Tile getStartTile() { return startTile != null ? startTile: (startTile = tileMap.getDefaultStartTile()); }
    public Tile getGoalTile() { return goalTile != null ? goalTile: (goalTile = tileMap.getDefaultGoalTile()); }
    public ArrayList<Tile> getSolution() { return pathSearch.getSolution(); }

    public boolean isGoalFound() { return pathSearch.isDone(); }

    String checkSolution()
    {
        if (pathSearch.isDone())
        {
            ArrayList<Tile> solution = pathSearch.getSolution();

            if (solution != null && !solution.isEmpty())
            {
                if (solution.get(solution.size() - 1) != startTile)
                    return "The first tile is not the start!";

                if (solution.get(0) != goalTile)
                    return "The last tile is not the goal!";

                // Calculate and store the tile distance up front.
                double sqDiameter = tileMap.getTileRadius() * tileMap.getTileRadius() * 4;

                for (int index = 0; index < solution.size() - 1; index++)
                {
                    double sqDistance = tileDistanceSquared(solution.get(index), solution.get(index+1));
                    double distanceError = sqDistance - sqDiameter;

                    if (distanceError > Math.ulp(Math.max(sqDistance, sqDiameter)) * 256 || distanceError < Math.ulp(Math.max(sqDistance, sqDiameter)) * -256)
                    {
                        System.out.println("distanceError: [" + distanceError + "]; sqDist: [" + sqDistance + "]; ulp: [" + Math.ulp(Math.max(sqDistance, sqDiameter)) * 256 + "].");
                        return "A node is not next to its parent!";
                    }
                }
            }
            else
                return "isDone() returned true and getSolution() returned a vector size 0";
        }
        return null;
    }

    static double tileDistanceSquared(Tile lhs, Tile rhs)
    {
        double dx = lhs.getX() - rhs.getX();
        double dy = lhs.getY() - rhs.getY();
        return dx * dx + dy * dy;
    }

}
