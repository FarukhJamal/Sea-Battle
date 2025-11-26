using System;

public class PlayerController : IPlayerController
{
    public IGrid Grid { get; private set; }
    private int partsToPlace;

    public int RemainingPlacementParts => partsToPlace;

    public PlayerController(IGrid grid, int partsToPlace)
    {
        Grid = grid;
        this.partsToPlace = partsToPlace;
    }

    // Called from UI placement clicks
    public void PlaceShipPart(int x, int y)
    {
        if (partsToPlace <= 0) return;

        var cell = Grid.GetCell(x, y);
        if (cell.HasShip) return;

        cell.HasShip = true;
        partsToPlace = Math.Max(0, partsToPlace - 1);
    }
}
