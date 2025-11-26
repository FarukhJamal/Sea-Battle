public enum Orientation { Horizontal, Vertical }

public class Ship
{
    public int Length { get; }
    public Orientation Orientation { get; private set; }
    public int AnchorX { get; private set; }
    public int AnchorY { get; private set; }

    public Ship(int length)
    {
        Length = length;
        Orientation = Orientation.Horizontal;
    }

    public void Rotate() => Orientation = (Orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal;

    public void SetPosition(int anchorX, int anchorY)
    {
        AnchorX = anchorX; AnchorY = anchorY;
    }
}
