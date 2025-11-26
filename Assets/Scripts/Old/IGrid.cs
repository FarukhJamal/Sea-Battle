public interface IGrid
{
    int Width { get; }
    int Height { get; }
    IGridCell GetCell(int x, int y);
    bool IsAllShipsSunk();
}
