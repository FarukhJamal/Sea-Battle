using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Cell : MonoBehaviour, IDropHandler
{
    public int x;
    public int y;

    public bool hasShipPart = false;
    public bool isHit = false;

    private BoardManager owner;

    [Header("Visual References")]
    public GameObject shipVisual;
    public GameObject hitMarker;
    public GameObject missMarker;
    public GameObject highlightMarker;

    public void Init(int x, int y, BoardManager owner)
    {
        this.x = x;
        this.y = y;
        this.owner = owner;

        UpdateVisual();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var shipDrag = eventData.pointerDrag?.GetComponent<ShipDragHandler>();
        if (shipDrag != null && owner != null)
        {
            // Place the ship at this cell
            owner.PlaceShipFromCell(shipDrag, this);

            // Attach undo click handler if not already present
            if (shipDrag.gameObject.GetComponent<ShipClickUndo>() == null)
            {
                var undo = shipDrag.gameObject.AddComponent<ShipClickUndo>();
                undo.Init(shipDrag, shipDrag.GetComponent<Ship>(), owner);
            }
        }
    }

    public void UpdateVisual()
    {
        if (shipVisual != null)
            shipVisual.SetActive(hasShipPart);

        if (hitMarker != null)
            hitMarker.SetActive(isHit && hasShipPart);

        if (missMarker != null)
            missMarker.SetActive(isHit && !hasShipPart);

        if (highlightMarker != null)
            highlightMarker.SetActive(false);
    }

    public void SetHighlight(bool on)
    {
        if (highlightMarker != null)
            highlightMarker.SetActive(on);
    }
}
