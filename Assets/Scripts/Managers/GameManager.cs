using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public PlayerController playerController;
    public BoardManager playerBoard;
    public BoardManager aiBoard; // hidden until needed

    private ShipPlacementData playerPlacementData;
    private ShipPlacementData aiPlacementData;

    private void Start()
    {
        // 1. Create boards
        playerBoard?.CreateGrid();
        aiBoard?.CreateGrid();

        // 2. Bind UI buttons
        if (uiManager != null)
        {
            uiManager.OnRandomClicked += OnRandomPlacement;
            uiManager.OnSaveClicked += OnSavePlacement;
        }

        // 3. Start player placement
        StartPlayerPlacement();
    }

    private void StartPlayerPlacement()
    {
        uiManager?.ShowPlayerPlacementPanel();
        if (playerController != null) playerController.playerBoard = playerBoard;
        playerController?.StartPlacement();
    }

    /// <summary>
    /// Randomly places all ships on the player's board.
    /// </summary>
    private void OnRandomPlacement()
    {
        if (playerController == null || playerBoard == null) return;

        playerBoard.ClearBoard(); // clear current placements

        foreach (Transform t in playerController.shipsPanel)
        {
            var ship = t.GetComponent<Ship>();
            if (ship == null) continue;

            ship.isPlaced = false;
            ship.occupiedCells.Clear();

            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 200)
            {
                attempts++;

                // Random orientation
                ship.orientation = Random.value > 0.5f ? ShipOrientation.Horizontal : ShipOrientation.Vertical;

                // Random starting cell
                int startX = Random.Range(0, playerBoard.rows);
                int startY = Random.Range(0, playerBoard.columns);

                if (!playerBoard.CanPlaceShip(startX, startY, ship.size, ship.orientation))
                    continue;

                // Mark cells manually
                int dx = ship.orientation == ShipOrientation.Horizontal ? 1 : 0;
                int dy = ship.orientation == ShipOrientation.Vertical ? 1 : 0;

                for (int i = 0; i < ship.size; i++)
                {
                    int cx = startX + i * dx;
                    int cy = startY + i * dy;
                    var cell = playerBoard.GetCell(cx, cy);
                    if (cell != null)
                    {
                        cell.hasShipPart = true;
                        cell.UpdateVisual();
                        ship.occupiedCells.Add(cell);
                    }
                }

                ship.isPlaced = true;

                // Move ship UI to match top-left cell
                var startCellPos = playerBoard.GetCell(startX, startY)?.GetComponent<RectTransform>().position ?? t.position;
                t.position = startCellPos;

                placed = true;
            }

            if (!placed)
                Debug.LogWarning($"Could not randomly place ship {ship.name} after {attempts} attempts.");
        }
    }

    /// <summary>
    /// Saves the current placement of player and AI ships.
    /// </summary>
    private void OnSavePlacement()
    {
        if (playerBoard != null)
            playerPlacementData = new ShipPlacementData(playerBoard);

        if (aiBoard != null)
        {
            RandomlyPlaceAIShips();
            aiPlacementData = new ShipPlacementData(aiBoard);
        }

        uiManager?.ShowCounter(ShowGameplayBoards);
    }

    /// <summary>
    /// Randomly place AI ships on its board.
    /// </summary>
    private void RandomlyPlaceAIShips()
    {
        playerController.RandomlyPlaceShips();
    }

    /// <summary>
    /// Populate a board from saved placement data.
    /// </summary>
    public void PopulateBoardFromPlacement(BoardManager targetBoard, ShipPlacementData data, bool revealShips)
    {
        targetBoard.ClearBoard();

        foreach (var shipData in data.ships)
        {
            foreach (var coord in shipData.occupiedCells)
            {
                var cell = targetBoard.GetCell(coord.x, coord.y);
                if (cell != null)
                {
                    cell.hasShipPart = true;
                    if (cell.shipVisual != null) cell.shipVisual.SetActive(revealShips);
                    cell.UpdateVisual();
                }
            }
        }
    }

    /// <summary>
    /// Called after both player & AI placements saved.
    /// </summary>
    public void ShowGameplayBoards()
    {
        if (playerBoard != null && aiBoard != null)
        {
            PopulateBoardFromPlacement(playerBoard, playerPlacementData, true);
            PopulateBoardFromPlacement(aiBoard, aiPlacementData, false);
        }

        uiManager?.ShowGameplayPanel();
    }
}
