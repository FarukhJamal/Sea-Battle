// ShipPlacer.cs
using UnityEngine;

/// <summary>
/// Simple ship placer: places a number of single-tile parts randomly on a grid model.
/// (We keep it simple: the "shipParts" count is the total single-tile parts to place.)
/// If you want multi-tile ships or drag-and-drop placement, extend this script later.
/// </summary>
public class ShipPlacer : MonoBehaviour
{
    public void PlaceRandomParts(GridModel model, int shipParts)
    {
        // clear first
        for (int x = 0; x < model.Width; x++)
            for (int y = 0; y < model.Height; y++)
            {
                model.GetCell(x, y).HasShip = false;
                model.GetCell(x, y).Attacked = false;
            }

        int placed = 0;
        int tries = 0;
        var rnd = new System.Random();

        while (placed < shipParts && tries < 10000)
        {
            tries++;
            int x = rnd.Next(0, model.Width);
            int y = rnd.Next(0, model.Height);
            var c = model.GetCell(x, y);
            if (!c.HasShip)
            {
                c.HasShip = true;
                placed++;
            }
        }
    }
}
