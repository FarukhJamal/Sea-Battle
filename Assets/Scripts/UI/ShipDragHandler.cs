using UnityEngine;
using UnityEngine.EventSystems;

public class ShipDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup cg;
    private Ship ship;
    private BoardManager boardManager;

    private Vector2 pointerOffset;
    private Vector2 originalAnchoredPos;

    public bool canDrag = true;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        ship = GetComponent<Ship>();
        cg = GetComponent<CanvasGroup>();

        if (cg != null)
            cg.blocksRaycasts = true; // always true
    }

    public void Init(BoardManager board)
    {
        boardManager = board;
        canvas = GetComponentInParent<Canvas>();
        ship.originalSpawnPos = rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        originalAnchoredPos = rect.anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPointer
        );
        pointerOffset = rect.anchoredPosition - localPointer;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPointer
        );

        rect.anchoredPosition = localPointer + pointerOffset;

        if (boardManager != null)
            boardManager.HighlightPlacement(ship, rect);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        bool placed = boardManager != null && boardManager.TryPlaceShip(ship, rect);

        if (!placed)
        {
            rect.anchoredPosition = originalAnchoredPos;
            boardManager?.ClearHighlights();
        }
        else
        {
            canDrag = false; // prevent further dragging
            boardManager.ClearHighlights();

            // attach undo click
            if (gameObject.GetComponent<ShipClickUndo>() == null)
            {
                var clickUndo = gameObject.AddComponent<ShipClickUndo>();
                clickUndo.Init(this, ship, boardManager);
            }
        }
    }

    public void ResetPosition()
    {
        rect.anchoredPosition = ship.originalSpawnPos;
    }
}
