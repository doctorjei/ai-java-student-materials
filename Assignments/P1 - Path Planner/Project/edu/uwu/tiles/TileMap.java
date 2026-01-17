package edu.uwu.tiles;

import java.util.Scanner;
import java.io.InputStream;

/**
 * Logical representation of a tile map. All tile information can be accessed from here,
   where it is stored as a 2D grid. Note: do not construct your own TileMap objects.
 *
 * @author Cromwell D. Enage, 2008, 2010
 * @author Jeremiah Blanchard, 2016, 2017, 2021, 2025
 */
public class TileMap
{
    private int rowCount = 0, columnCount = 0;
    private Tile startTile = null, goalTile = null;
    private Tile[][] tiles = null;
    private double tileRadius = 0;

    public TileMap() { }                                 // Default constructor.
    public TileMap(TileMap source) { copyFrom(source); } // Copy constructor.

    public Tile getDefaultStartTile() { return startTile; }    // Returns default starting tile.
    public Tile getDefaultGoalTile() { return goalTile; }      // Returns default goal tile
    public int getRowCount() { return rowCount; }       // Returns number of rows.
    public int getColumnCount() { return columnCount; } // Returns number of columns.

    public double getTileRadius() { return tileRadius; } // Returns inner-radius of tiles.

    // Returns reference to tile at specified location.
    public Tile getTile(int row, int column)
    {
        if (tiles != null && (row >= 0) && (column >= 0)
          && (row < rowCount) && (column < columnCount))
            return tiles[row][column];
        else
            return null;
    }

    // Sets default start / goal tile (by row and column).
    public void setStartTile(int row, int column) { startTile = getTile(row, column); }
    public void setGoalTile(int row, int column) { goalTile = getTile(row, column); }

    /**
     * Sets radius of largest circle that can be circumscribed by a tile and radius of any tiles
     * previously created. All search algorithms using this tile map must be reset after invoking.
     */
    public void setRadius(double radius)
    {
        tileRadius = radius;

        if (tiles == null)
            return;

        for (int row = 0; row < rowCount; row++)
            for (int col = 0; col < columnCount; col++)
                if (tiles[row][col] != null)
                    tiles[row][col].setRadius(radius);
    }

    // Creates and adds a Tile object at specified location with specified weight.
    public void addTile(int row, int column, int weight)
    {
        tiles[row][column] = new Tile(row, column, tileRadius, weight);
    }

    // Resets all drawing colors set in the tiles to transparent black (0x00000000).
    public void resetTileDrawing()
    {
        if (tiles == null)
            return;

        for (int row = 0; row < rowCount; row++)
            for (int col = 0; col < columnCount; col++)
                if (tiles[row][col] != null)
                    tiles[row][col].resetDrawing();
    }

    // Assignment operator equivalent.
    public void copyFrom(TileMap source)
    {
        if (this == source || source == null || source.tiles == null)
            return;

        rowCount = source.rowCount;
        columnCount = source.columnCount;
        tileRadius = source.tileRadius;
        tiles = new Tile[rowCount][columnCount];

        for (int row = 0; row < rowCount; row++)
            for (int column = 0; column < columnCount; column++)
                if (source.tiles[row][column] != null)
                    tiles[row][column] = new Tile(row, column,
                        tileRadius, source.tiles[row][column].getWeight());

        // Set start and goal tiles if they exist in the original
        if (source.startTile != null)
            startTile = getTile(source.startTile.getRow(), source.startTile.getColumn());

        if (source.goalTile != null)
            goalTile = getTile(source.goalTile.getRow(), source.goalTile.getColumn());
    }

    /**
     * Loads into this tilemap from specified input stream.
     *
     * @param inStream the input stream to read from
     * @return true if loading was successful, false otherwise
     */
    public boolean loadFromStream(InputStream inStream)
    {
        try (Scanner scanner = new Scanner(inStream))
        {
            // Try to get the size. If it's missing, return without changes.
            int rows = scanner.hasNextInt() ? scanner.nextInt() : -1;
            int columns = scanner.hasNextInt() ? scanner.nextInt() : -1;

            if (rows < 0 || columns < 0)
                return false;

            reset();
            rowCount = rows;
            columnCount = columns;
            tiles = new Tile[rows][columns];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    // Read the data; if its absent invalid, reset and abort.
                    int data = scanner.hasNextInt() ? scanner.nextInt() : -1;

                    if (data < 0 || data > 255)
                    {
                        reset();
                        return false;
                    }
                    addTile(row, column, data);
                }
            }

            int startRow = scanner.hasNextInt() ? scanner.nextInt() : 0;
            int startColumn = scanner.hasNextInt() ? scanner.nextInt() : 0;
            int goalRow = scanner.hasNextInt() ? scanner.nextInt() : rowCount - 1;
            int goalColumn = scanner.hasNextInt() ? scanner.nextInt() : columnCount - 1;

            setStartTile(startRow, startColumn);
            setGoalTile(goalRow, goalColumn);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    /**
     * Releases underlying tiles and zeroes row & column counts and  tile radius. All
     * search algorithms using this tile map must be reset after invoking this method.
     */
    public void reset()
    {
        tiles = null;
        startTile = goalTile = null;
        rowCount = columnCount = 0;
        tileRadius = 0.0;
    }
}
