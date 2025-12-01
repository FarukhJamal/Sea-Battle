using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardState
{
    // Simple representation: list of occupied coordinates (x,y)
    public List<Vector2Int> occupiedCells = new List<Vector2Int>();

    // Optionally extend to store ships with ids/sizes:
    [System.Serializable]
    public class ShipSave
    {
        public string shipId;
        public List<Vector2Int> coords = new List<Vector2Int>();
    }
    public List<ShipSave> ships = new List<ShipSave>();
}
