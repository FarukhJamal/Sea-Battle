// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // optional

/// <summary>
/// Minimal UI manager: holds references and provides view-update methods.
/// UI-only; no game logic here.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject placementPanel;
    public GameObject battlePanel;
    public GameObject resultPanel;

    [Header("Text")]
    public TextMeshProUGUI placementCounterText; // optional (use Text if you prefer)
    public TextMeshProUGUI resultText;

    [Header("GridGenerators (assign two separate instances)")]
    public GridGenerator playerGridGenerator;
    public GridGenerator aiGridGenerator;

    // Views arrays (populated by GameManager)
    public GridCellView[,] PlayerViews { get; set; }
    public GridCellView[,] AIViews { get; set; }

    public void ShowPlacementUI()
    {
        placementPanel?.SetActive(true);
        battlePanel?.SetActive(false);
        resultPanel?.SetActive(false);
    }

    public void ShowBattleUI()
    {
        placementPanel?.SetActive(false);
        battlePanel?.SetActive(true);
        resultPanel?.SetActive(false);
    }

    public void ShowResult(string text)
    {
        resultPanel?.SetActive(true);
        resultText.text = text;
    }

    public void UpdatePlacementCounter(int remaining)
    {
        if (placementCounterText != null)
            placementCounterText.text = $"Place Parts: {remaining}";
    }

    // Visual updates called by GameManager when model changes:
    public void MarkPlayerCellHit(int x, int y) => PlayerViews[x, y].ShowHit();
    public void MarkPlayerCellMiss(int x, int y) => PlayerViews[x, y].ShowMiss();
    public void MarkPlayerShipVisible(int x, int y, bool visible) => PlayerViews[x, y].SetShipVisible(visible);

    public void MarkAICellHit(int x, int y) => AIViews[x, y].ShowHit();
    public void MarkAICellMiss(int x, int y) => AIViews[x, y].ShowMiss();
}
