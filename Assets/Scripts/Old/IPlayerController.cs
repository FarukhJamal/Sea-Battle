public interface IPlayerController
{
    IGrid Grid { get; }
    int RemainingPlacementParts { get; }
    void PlaceShipPart(int x, int y);
}
