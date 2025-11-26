using System;

public class GridModel
{
    private readonly GridCell[,] cells;
    public int Width { get; }
    public int Height { get; }

    public GridModel(int width, int height)
    {
        if (width <= 0 || height <= 0) throw new ArgumentException("Invalid grid size");
        Width = width;
        Height = height;
        cells = new GridCell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                cells[x, y] = new GridCell(x, y);
    }

    public GridCell GetCell(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException();
        return cells[x, y];
    }

    public bool IsAllShipsSunk()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (cells[x, y].HasShip && !cells[x, y].Attacked) return false;
        return true;
    }
}
