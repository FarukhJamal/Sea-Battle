using System.Collections.Generic;
using UnityEngine;


public class PlacedShipData
{
    public int size;
    public ShipOrientation orientation;
    public List<Vector2Int> occupiedCells = new List<Vector2Int>();


    public PlacedShipData(Ship s)
    {
        size = s.size;
        orientation = s.orientation;
        foreach (var c in s.occupiedCells)
        {
            occupiedCells.Add(new Vector2Int(c.x, c.y));
        }
    }
}


public class ShipPlacementData
{
    public List<PlacedShipData> ships = new List<PlacedShipData>();


    public ShipPlacementData() { }


    public ShipPlacementData(BoardManager board)
    {
        foreach (var ship in board.GetPlacedShips())
            ships.Add(new PlacedShipData(ship));
    }
}