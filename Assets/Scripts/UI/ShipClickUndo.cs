using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipClickUndo : MonoBehaviour, IPointerClickHandler
{
    private ShipDragHandler dragHandler;
    private Ship ship;
    private BoardManager boardManager;

    public void Init(ShipDragHandler handler, Ship s, BoardManager board)
    {
        Debug.Log("Init" + handler);
        dragHandler = handler;
        ship = s;
        boardManager = board;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ship == null || dragHandler == null || boardManager == null) return;

        // remove ship from cells
        foreach (var c in ship.occupiedCells)
        {
            c.hasShipPart = false;
            c.UpdateVisual();
        }

        // remove from board
        var listField = boardManager.GetType()
            .GetField("placedShips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var list = listField?.GetValue(boardManager) as List<Ship>;
        list?.Remove(ship);

        // reset ship
        ship.occupiedCells.Clear();
        ship.isPlaced = false;
        dragHandler.canDrag = true;
        ship.GetComponent<RectTransform>().anchoredPosition = ship.originalSpawnPos;

        Destroy(this);
    }

}
