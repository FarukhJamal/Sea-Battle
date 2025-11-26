using System;

public class SimpleAIController : IAIController
{
    public IGrid Grid { get; private set; }
    private readonly int partsToPlace;
    private readonly IRandomProvider random;

    public SimpleAIController(IGrid grid, int partsToPlace, IRandomProvider randomProvider)
    {
        Grid = grid;
        this.partsToPlace = Math.Max(1, partsToPlace);
        random = randomProvider;
    }

    public void PlaceShips()
    {
        int placed = 0;
        while (placed < partsToPlace)
        {
            int x = random.Range(0, Grid.Width);
            int y = random.Range(0, Grid.Height);
            var c = Grid.GetCell(x, y);
            if (!c.HasShip)
            {
                c.HasShip = true;
                placed++;
            }
        }
    }

    public (int x, int y) SelectAttackTarget(IGrid opponentGrid)
    {
        int x, y;
        do
        {
            x = random.Range(0, opponentGrid.Width);
            y = random.Range(0, opponentGrid.Height);
        }
        while (opponentGrid.GetCell(x, y).Attacked);

        return (x, y);
    }
}
