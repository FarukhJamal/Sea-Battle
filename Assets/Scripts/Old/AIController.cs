using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Controller selects targets to attack. 
/// Current implementation: random undiscovered cells.
/// It exposes a simple queue for hunting mode (can be used later).
/// </summary>
public class AIController
{
    private readonly GridModel playerGrid;
    private readonly System.Random rng = new System.Random();
    private readonly Queue<(int x, int y)> huntQueue = new Queue<(int x, int y)>();

    public AIController(GridModel playerGridModel)
    {
        playerGrid = playerGridModel;
    }

    public (int x, int y) GetNextTarget()
    {
        // Use queued hunt targets first
        while (huntQueue.Count > 0)
        {
            var t = huntQueue.Dequeue();
            if (!playerGrid.GetCell(t.x, t.y).Attacked) return t;
        }

        // Random selection of an unattacked cell
        int attempts = 0;
        while (attempts < 10000)
        {
            attempts++;
            int x = rng.Next(0, playerGrid.Width);
            int y = rng.Next(0, playerGrid.Height);
            if (!playerGrid.GetCell(x, y).Attacked) return (x, y);
        }

        // fallback: search brute-force
        for (int x = 0; x < playerGrid.Width; x++)
            for (int y = 0; y < playerGrid.Height; y++)
                if (!playerGrid.GetCell(x, y).Attacked) return (x, y);

        return (-1, -1); // no target available
    }

    /// <summary>
    /// When AI hits a player's ship, call this to queue neighbors for hunt mode.
    /// </summary>
    public void EnqueueNeighbors(int hx, int hy)
    {
        var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
        foreach (var d in dirs)
        {
            int nx = hx + d.dx, ny = hy + d.dy;
            if (nx >= 0 && nx < playerGrid.Width && ny >= 0 && ny < playerGrid.Height)
                if (!playerGrid.GetCell(nx, ny).Attacked)
                    huntQueue.Enqueue((nx, ny));
        }
    }
}
