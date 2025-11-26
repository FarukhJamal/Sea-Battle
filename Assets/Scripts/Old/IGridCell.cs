public interface IGridCell
{
    int X { get; }
    int Y { get; }
    bool HasShip { get; set; }
    bool Attacked { get; set; }
}
