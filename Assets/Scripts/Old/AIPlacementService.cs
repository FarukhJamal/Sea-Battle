using UnityEngine;

public class AIPlacementService
{
    public void PlaceShips(IGrid aiGrid, int shipParts)
    {
        int placed = 0;
        int tries = 0;
        int maxTries = 5000;

        while (placed < shipParts && tries < maxTries)
        {
            tries++;
            int x = Random.Range(0, aiGrid.Width);
            int y = Random.Range(0, aiGrid.Height);

            var cell = aiGrid.GetCell(x, y);
            if (!cell.HasShip)
            {
                cell.HasShip = true;
                placed++;
            }
        }
    }
}
