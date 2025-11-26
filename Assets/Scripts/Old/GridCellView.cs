
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Attach this to the cell prefab (Button + Image). It forwards clicks and updates visuals.
/// UI-only: does not contain game logic.
/// </summary>
[RequireComponent(typeof(Button))]
public class GridCellView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject shipVisual; // visible only on player's grid
    [SerializeField] private GameObject hitVisual;
    [SerializeField] private GameObject missVisual;

    private int x;
    private int y;
    private bool isPlayerGrid;
    private Action<int, int, bool> clickCallback;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public void Initialize(int x, int y, bool isPlayerGrid, Action<int, int, bool> onClick)
    {
        this.x = x;
        this.y = y;
        this.isPlayerGrid = isPlayerGrid;
        this.clickCallback = onClick;

        ResetView();
    }

    public void ResetView()
    {
        shipVisual?.SetActive(false);
        hitVisual?.SetActive(false);
        missVisual?.SetActive(false);
        if (background != null) background.color = Color.white;
    }

    public void SetShipVisible(bool visible)
    {
        if (isPlayerGrid)
            shipVisual?.SetActive(visible);
    }

    public void ShowHit()
    {
        hitVisual?.SetActive(true);
        missVisual?.SetActive(false);
    }

    public void ShowMiss()
    {
        missVisual?.SetActive(true);
        hitVisual?.SetActive(false);
    }

    private void OnClicked()
    {
        clickCallback?.Invoke(x, y, isPlayerGrid);
    }
}
