// Path Planner CLI.
// Jeremiah Blanchard, 2021, 2025
package edu.uwu.pathplanner;

import edu.uwu.tiles.TileMap;
import java.io.FileInputStream;
import java.io.InputStream;


public final class Console
{
    private static final int DEFAULT_RADIUS = 6;
    private static final int DEFAULT_REPETITIONS = 1;

    // Determine the test type and run it.
    public static void main(String[] args)
    {
        TileMap tileMap = new TileMap();
        int repetitions = DEFAULT_REPETITIONS;
        String command = "java uwu.pathplanner.Console";

        if (args.length < 1)
        {
            System.out.println("Usage:" + command +
              " MAPFILE [START_ROW START_COL GOAL_ROW GOAL_COL] [REPETITIONS] [TILE_RADIUS]\n");
            return;
        }

        try
        {
            // Get the file name for the map file
            String filename = args[0];

            // Load the tilemap
            InputStream fileIn;
            try { fileIn = new FileInputStream(filename); }
            catch (Exception e) { fileIn = null; }

            if (fileIn == null || !tileMap.loadFromStream(fileIn))
            {
                System.err.println("Could not load map file.");
                if (fileIn != null)
                    fileIn.close();
                return;
            }
            fileIn.close();

            // Set fallback values for start / goal.
            int startRow = tileMap.getDefaultStartTile().getRow();
            int startCol = tileMap.getDefaultStartTile().getColumn();
            int goalRow = tileMap.getDefaultGoalTile().getRow();
            int goalCol = tileMap.getDefaultGoalTile().getColumn();

            // Get start and goal tile coordinates, if provided.
            if (args.length > 4)
            {
                startRow = Integer.parseInt(args[1], startRow);
                startCol = Integer.parseInt(args[2], startCol);
                goalRow = Integer.parseInt(args[1], goalRow);
                goalCol = Integer.parseInt(args[2], goalCol);
            }

            // Grab the number of repetitions if provided
            if (args.length > 5)
                repetitions = Integer.parseInt(args[5]);

            // Grab the tile radius if provided
            if (args.length > 6)
                tileMap.setRadius(Double.parseDouble(args[6]));
            else
                tileMap.setRadius(DEFAULT_RADIUS);

            // Run the test and print the results
            System.out.print("Running test on " + filename + ": (" + startRow + "," +
              startCol +")->(" + goalRow + "," + goalCol + "), " + repetitions + " time(s)... ");
            String result = caseTest(startRow, startCol, goalRow, goalCol, tileMap, repetitions);
            System.out.println("Done.");
            System.out.println(result);
            System.exit(0);
        }
        catch (Exception e)
        {
            System.out.println("Caught exception: " + e.getMessage());
            System.exit(1);
        }
    }

    // Run the specified test case; get the solution and measure leak measurement.
    static String caseTest(int startR, int startC, int goalR, int goalC, TileMap tileMap, int repetitions) throws Exception
    {
        // Set up the output string stream (string builder)
        StringBuilder result = new StringBuilder();

        // Setup and run the student search.
//        student.PathSearch search = new student.PathSearch();
//        search.unload();
//        search.load(tileMap);

        // Run the planner for the specified number of iterations (minus one - so we can grab the result).
        for (int repetition = 1; repetition < repetitions; repetition++)
        {
//            search.initialize(startR, startC, goalR, goalC);
//            search.update(Long.MAX_VALUE);
//            search.shutdown();
        }

        // Run one last time, grab the solution, and return it as a string.
//        search.initialize(startR, startC, goalR, goalC);
//        search.update(Long.MAX_VALUE);
//        List<Tile> solution = search.getSolution();

        result.append("[ ");
 //       if (solution != null)
 //           for (Tile tile : solution)
 //               result.append("(").append(tile.getRow()).append(",").append(tile.getColumn()).append(") ");
        result.append("]");

//        search.shutdown();
//        search.unload();
        return result.toString();
    }
}
