package edu.uwu.tiles;

import java.util.ArrayList;

/**
 * Logical representation of a tile in a hexagonal grid.
 *
 * Once a tile map is loaded, application displays each tile as either an obstacle (if its weight
 *  is zero) or a white/gray hexagon (otherwise). The smaller the weight, the darker the gray.
 *
 * @author Cromwell D. Enage, 2008
 * @author Jeremiah Blanchard, 2012, 2016, 2025
 */
public class Tile
{
    public static class Line
    {
        public final Tile endpoint;
        public final int color;

        public Line(Tile _endpoint, int _color)
        {
            endpoint = _endpoint;
            color = _color;
        }
    }

    int weight; // originally unsigned char
    int row, column;
    double x, y;

    // For drawing purposes
    int markerColor = 0, outlineColor = 0, fillColor = 0;
    ArrayList<Line> lines = new ArrayList<>();

    public Tile(int r, int c, double radius, int _weight)
    {
        weight = _weight;
        row = r;
        column = c;

        x = (((r & 1) != 0 ? ((c + 1) << 1) : ((c << 1) | 1)) * radius);
        y = ((r * 3 + 2) * radius / Math.sqrt(3.0));
    }

    // Getters
    public int getRow() { return row; }       // Returns the row-coordinate of this tile.
    public int getColumn() { return column; } // Returns the column-coordinate of this tile.
    public int getWeight() { return weight; } // Returns terrain weight of tile or zero if impassable.
    public double getX() { return x; }        // Returns the x-coordinate of this tile.
    public double getY() { return y; }        // Returns the y-coordinate of this tile.
    public int getMarker() { return markerColor; }   // Returns the LRGB marker color of this tile.
    public int getOutline() { return outlineColor; } // Returns the LRGB outline color of this tile.
    public int getFill() { return fillColor; }       // Returns the LRGB fill color of this tile.

    // Returns the list of lines being drawn from this tile.
    public ArrayList<Line> getLines() { return new ArrayList<>(lines); } // Return defensive copy

    // Setters - Sets this tile's marker / outline / fill color as designated in the LRGB color space.
    public void setMarker(int color) { markerColor = convertColorModel(color); }
    public void setOutline(int color) { outlineColor = convertColorModel(color); }
    public void setFill(int color) { fillColor = convertColorModel(color); }

    // Sets the radius and recalculates coordinates
    void setRadius(double radius)
    {
        x = ((row & 1) != 0 ? ((column + 1) << 1) : ((column << 1) | 1)) * radius;
        y = (row * 3 + 2) * radius / Math.sqrt(3.0);
    }

    /**
     * Adds a line to be drawn from this tile to destination as designated in the LRGB color space
     * if a line is not already being drawn to the destination from this tile.
     */
    public void addLineTo(Tile endpoint, int color)
    {
        lines.add(new Line(endpoint, convertColorModel(color)));
    }

    public void resetDrawing()
    {
        markerColor = outlineColor = fillColor = 0;
        lines.clear();
    }

    // Removes existing lines from the drawing set.
    public void clearLines() { lines.clear(); }

    // Converts between SBGR and LRGB
    private static int convertColorModel(int color)
    {
        int first = ~(color >> 24) & 0xFF;
        int second = color & 0xFF;
        int third = (color >> 8) & 0xFF;
        int fourth = (color >> 16) & 0xFF;

        return (first << 24) | (second << 16) | (third << 8) | fourth;
    }
}
