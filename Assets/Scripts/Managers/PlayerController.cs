using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BoardManager playerBoard;
    public RectTransform shipsPanel; // parent for ship UI objects
    public List<Ship> shipPrefabs = new List<Ship>(); // prefabs with Ship component
    public List<Transform> shipSpawnPoints = new List<Transform>();

    private List<Ship> instantiatedShips = new List<Ship>();

    /// <summary>
    /// Called to start the manual placement phase.
    /// </summary>
    public void StartPlacement()
    {
        InstantiateShipsToPanel();
    }

    /// <summary>
    /// Randomly places all ships on the player's board using drag-drop logic.
    /// </summary>
    public void RandomlyPlaceShips()
    {
        if (playerBoard == null || shipsPanel == null) return;

        playerBoard.ClearBoard();

        foreach (Transform t in shipsPanel)
        {
            Ship ship = t.GetComponent<Ship>();
            if (ship == null) continue;

            ship.isPlaced = false;
            ship.occupiedCells.Clear();

            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 500)
            {
                attempts++;

                // Random orientation
                ship.orientation = Random.value > 0.5f ? ShipOrientation.Horizontal : ShipOrientation.Vertical;

                int startRow, startCol;

                if (ship.orientation == ShipOrientation.Horizontal)
                {
                    startRow = Random.Range(0, playerBoard.rows);
                    int maxStartCol = playerBoard.columns - ship.size;
                    if (maxStartCol < 0) break;
                    startCol = Random.Range(0, maxStartCol + 1);
                }
                else
                {
                    startCol = Random.Range(0, playerBoard.columns);
                    int maxStartRow = playerBoard.rows - ship.size;
                    if (maxStartRow < 0) break;
                    startRow = Random.Range(0, maxStartRow + 1);
                }

                // safety check
                if (!playerBoard.CanPlaceShip(startRow, startCol, ship.size, ship.orientation))
                    continue;

                // register logically (marks cells.hasShipPart and ship.occupiedCells)
                bool ok = playerBoard.RegisterPlacedShip(ship, startRow, startCol);
                if (!ok) continue;

                // SNAP VISUALLY: reparent ship to gridParent and set anchoredPosition to the cell's anchoredPosition
                var firstCell = playerBoard.GetCell(startRow, startCol);
                if (firstCell != null)
                {
                    RectTransform cellRt = firstCell.GetComponent<RectTransform>();
                    RectTransform shipRt = ship.rectTransform;

                    if (cellRt != null && shipRt != null)
                    {
                        // Reparent to gridParent so they share the same local coordinate space
                        shipRt.SetParent(playerBoard.gridParent, worldPositionStays: false);

                        // Align anchoredPosition exactly to the cell
                        shipRt.anchoredPosition = cellRt.anchoredPosition;

                        // Optional: match rotation or pivot if required
                        shipRt.localRotation = Quaternion.identity;
                    }
                }

                ship.isPlaced = true;
                placed = true;
            }

            if (!placed)
                Debug.LogWarning($"[RandomPlacement] Could not place ship {ship.name} after {attempts} attempts.");
        }
    }





    /// <summary>
    /// Instantiates ships into the UI panel for manual placement.
    /// </summary>
    private void InstantiateShipsToPanel()
    {
        // Destroy old ships safely
        foreach (Transform t in shipsPanel)
        {
            if (t != null) Destroy(t.gameObject);
        }
        instantiatedShips.Clear();

        for (int i = 0; i < shipPrefabs.Count; i++)
        {
            var prefab = shipPrefabs[i];
            var go = Instantiate(prefab.gameObject, shipsPanel);
            var ship = go.GetComponent<Ship>();

            ship.isPlaced = false;
            ship.occupiedCells.Clear();

            // Set spawn position if provided
            if (shipSpawnPoints != null && i < shipSpawnPoints.Count)
            {
                go.transform.position = shipSpawnPoints[i].position;
            }

            // Resize UI to match board
            var rt = ship.rectTransform;
            rt.sizeDelta = new Vector2(playerBoard.cellSize.x * ship.size, playerBoard.cellSize.y);

            // Add / Init Drag Handler
            var drag = go.GetComponent<ShipDragHandler>();
            if (drag == null) drag = go.AddComponent<ShipDragHandler>();
            drag.Init(playerBoard);

            instantiatedShips.Add(ship);
        }
    }

    /// <summary>
    /// Returns all ships in the panel (placed or not).
    /// </summary>
    public IEnumerable<Ship> GetAllShipsInPanel()
    {
        List<Ship> ships = new List<Ship>();
        foreach (Transform t in shipsPanel)
        {
            var ship = t.GetComponent<Ship>();
            if (ship != null) ships.Add(ship);
        }
        return ships;
    }

    /// <summary>
    /// Returns all ships that have been placed on the board.
    /// </summary>
    public IEnumerable<Ship> GetPlacedShips() => instantiatedShips.FindAll(s => s.isPlaced);
}
