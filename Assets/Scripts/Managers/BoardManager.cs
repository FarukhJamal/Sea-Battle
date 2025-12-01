using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BoardManager : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 10;
    public int columns = 10;
    public Cell cellPrefab;
    public RectTransform gridParent;

    [Header("Cell Layout")]
    public Vector2 cellSize = new Vector2(64, 64);

    private Cell[,] cells;

    private List<Ship> placedShips = new List<Ship>();

    public Canvas mainCanvas;
    public GridLayoutGroup gridLayout;

    public void CreateGrid()
    {
        if (gridParent == null) gridParent = GetComponent<RectTransform>();

        for (int i = gridParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(gridParent.GetChild(i).gameObject);

        cells = new Cell[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                var go = Instantiate(cellPrefab.gameObject, gridParent);
                go.name = $"Cell_{x}_{y}";
                var cell = go.GetComponent<Cell>();
                cell.Init(x, y, this);
                cells[x, y] = cell;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= rows || y >= columns) return null;
        return cells[x, y];
    }

    public IEnumerable<Ship> GetPlacedShips() => placedShips;

    public void ClearHighlights()
    {
        if (cells == null) return;
        for (int x = 0; x < rows; x++)
            for (int y = 0; y < columns; y++)
                cells[x, y].SetHighlight(false);
    }

    public void HighlightPlacement(Ship ship, RectTransform shipUI)
    {
        ClearHighlights();
        if (ship == null || shipUI == null) return;

        if (!TryGetCellIndexFromScreenPos(shipUI.position, out int startX, out int startY)) return;

        int offsetX = (ship.orientation == ShipOrientation.Vertical) ? ship.size / 2 : 0;
        int offsetY = (ship.orientation == ShipOrientation.Horizontal) ? ship.size / 2 : 0;
        int startXCorrected = startX - offsetX;
        int startYCorrected = startY - offsetY;

        int dx = (ship.orientation == ShipOrientation.Horizontal) ? 1 : 0;
        int dy = (ship.orientation == ShipOrientation.Vertical) ? 1 : 0;

        for (int i = 0; i < ship.size; i++)
        {
            int cx = startXCorrected + i * dx;
            int cy = startYCorrected + i * dy;
            var c = GetCell(cx, cy);
            if (c != null)
                c.SetHighlight(true);
        }
    }

    public bool TryPlaceShip(Ship ship, RectTransform shipUI)
    {
        if (ship == null || shipUI == null) return false;

        if (!TryGetCellIndexFromScreenPos(shipUI.position, out int startX, out int startY))
            return false;

        int offsetX = (ship.orientation == ShipOrientation.Vertical) ? ship.size / 2 : 0;
        int offsetY = (ship.orientation == ShipOrientation.Horizontal) ? ship.size / 2 : 0;
        int startXCorrected = startX - offsetX;
        int startYCorrected = startY - offsetY;

        if (!CanPlaceShip(startXCorrected, startYCorrected, ship.size, ship.orientation))
            return false;

        int dx = (ship.orientation == ShipOrientation.Horizontal) ? 1 : 0;
        int dy = (ship.orientation == ShipOrientation.Vertical) ? 1 : 0;

        ship.occupiedCells.Clear();
        for (int i = 0; i < ship.size; i++)
        {
            int cx = startXCorrected + i * dx;
            int cy = startYCorrected + i * dy;
            var c = GetCell(cx, cy);
            c.hasShipPart = true;
            c.UpdateVisual();
            ship.occupiedCells.Add(c);
        }

        ship.isPlaced = true;
        ship.placedOrigin = new Vector2Int(startXCorrected, startYCorrected);
        placedShips.Add(ship);

        return true;
    }

    public bool CanPlaceShip(int startX, int startY, int size, ShipOrientation orientation)
    {
        if (orientation == ShipOrientation.Horizontal)
        {
            if (startY + size > columns) return false;
        }
        else
        {
            if (startX + size > rows) return false;
        }

        int dx = (orientation == ShipOrientation.Horizontal) ? 1 : 0;
        int dy = (orientation == ShipOrientation.Vertical) ? 1 : 0;

        for (int i = 0; i < size; i++)
        {
            int cx = startX + i * dx;
            int cy = startY + i * dy;
            var c = GetCell(cx, cy);
            if (c==null||c.hasShipPart) return false;
        }
        return true;
    }

    public void PlaceShipFromCell(ShipDragHandler shipDrag, Cell startCell)
    {
        Ship ship = shipDrag.GetComponent<Ship>();
        if (ship == null || startCell == null) return;

        int dx = (ship.orientation == ShipOrientation.Horizontal) ? 0 : 1;
        int dy = (ship.orientation == ShipOrientation.Horizontal) ? 1 : 0;

        for (int i = 0; i < ship.size; i++)
        {
            int cx = startCell.x + i * dx;
            int cy = startCell.y + i * dy;
            Cell c = GetCell(cx, cy);
            if (c == null || c.hasShipPart)
            {
                shipDrag.ResetPosition();
                return;
            }
        }

        ship.occupiedCells.Clear();
        for (int i = 0; i < ship.size; i++)
        {
            int cx = startCell.x + i * dx;
            int cy = startCell.y + i * dy;
            Cell c = GetCell(cx, cy);
            c.hasShipPart = true;
            c.UpdateVisual();
            ship.occupiedCells.Add(c);
        }

        ship.isPlaced = true;
        ship.placedOrigin = new Vector2Int(startCell.x, startCell.y);
        placedShips.Add(ship);

        Destroy(shipDrag);
    }

    public bool TryGetCellIndexFromScreenPos(Vector2 screenPos, out int x, out int y)
    {
        x = y = -1;
        if (gridParent == null || mainCanvas == null || gridLayout == null)
            return false;

        Camera cam = mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCanvas.worldCamera;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridParent, screenPos, cam, out Vector2 localPoint))
            return false;

        // Convert to top-left based local coordinate
        localPoint.x += gridParent.rect.width * 0.5f;
        localPoint.y = gridParent.rect.height * 0.5f - localPoint.y;

        float cellW = gridLayout.cellSize.x;
        float cellH = gridLayout.cellSize.y;

        float spacingX = gridLayout.spacing.x;
        float spacingY = gridLayout.spacing.y;

        float startX = gridLayout.padding.left;
        float startY = gridLayout.padding.top;

        // Out of grid area?
        if (localPoint.x < startX || localPoint.y < startY)
            return false;

        // Compute indexes including spacing
        int col = Mathf.FloorToInt((localPoint.x - startX) / (cellW + spacingX));
        int row = Mathf.FloorToInt((localPoint.y - startY) / (cellH + spacingY));

        if (col < 0 || row < 0 || col >= columns || row >= rows)
            return false;

        // Handle GridLayout corner orientation
        switch (gridLayout.startCorner)
        {
            case GridLayoutGroup.Corner.UpperRight:
                col = (columns - 1) - col;
                break;

            case GridLayoutGroup.Corner.LowerLeft:
                row = (rows - 1) - row;
                break;

            case GridLayoutGroup.Corner.LowerRight:
                col = (columns - 1) - col;
                row = (rows - 1) - row;
                break;
        }

        x = row;
        y = col;
        return true;
    }


    public void ClearBoard()
    {
        placedShips.Clear();
        if (cells == null) return;

        for (int x = 0; x < rows; x++)
            for (int y = 0; y < columns; y++)
            {
                cells[x, y].hasShipPart = false;
                cells[x, y].isHit = false;
                cells[x, y].UpdateVisual();
            }
    }
    // Public helper to register a ship at explicit grid indices (row = x, col = y).
    // Returns true if placed successfully.
    public bool RegisterPlacedShip(Ship ship, int startX, int startY)
    {
        if (ship == null) return false;

        // Validate bounds using same rules as CanPlaceShip
        if (!CanPlaceShip(startX, startY, ship.size, ship.orientation))
            return false;

        int dx = (ship.orientation == ShipOrientation.Horizontal) ? 1 : 0; // horizontal increments columns
        int dy = (ship.orientation == ShipOrientation.Vertical) ? 1 : 0;   // vertical increments rows
                                                                           // NOTE: We use the same mapping you have in other methods (row=x, col=y).
                                                                           // But to keep consistent with earlier code where dx/dy were swapped, let's derive exactly:
                                                                           // For your existing methods (TryPlaceShip / PlaceShipFromCell) you used:
                                                                           //   dx = (orientation == Horizontal) ? 1 : 0; for iterating the axis of orientation
                                                                           //   dy = (orientation == Vertical) ? 1 : 0;
                                                                           // To keep it simple here, compute per-step indices directly below.

        ship.occupiedCells.Clear();
        for (int i = 0; i < ship.size; i++)
        {
            int cx = startX + (ship.orientation == ShipOrientation.Vertical ? i : 0);
            int cy = startY + (ship.orientation == ShipOrientation.Horizontal ? i : 0);

            Cell c = GetCell(cx, cy);
            if (c == null || c.hasShipPart)
            {
                // Should not happen because CanPlaceShip passed, but guard anyway
                // rollback
                foreach (var occ in ship.occupiedCells)
                {
                    occ.hasShipPart = false;
                    occ.UpdateVisual();
                }
                ship.occupiedCells.Clear();
                return false;
            }

            c.hasShipPart = true;
            c.UpdateVisual();
            ship.occupiedCells.Add(c);
        }

        ship.isPlaced = true;
        ship.placedOrigin = new Vector2Int(startX, startY);

        // add to internal list if not already
        if (!placedShips.Contains(ship))
            placedShips.Add(ship);

        return true;
    }

}
