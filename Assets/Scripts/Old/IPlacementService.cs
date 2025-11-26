public interface IPlacementService
{
    void PlaceAIShips(IGrid aiGrid, int partsToPlace);
    // Player placement is handled by UI; method intentionally does not place player ships.
}
