public class GridCell : IGridCell
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool HasShip { get; set; }
    public bool Attacked { get; set; }

    public GridCell(int x, int y)
    {
        X = x;
        Y = y;
        HasShip = false;
        Attacked = false;
    }
}
