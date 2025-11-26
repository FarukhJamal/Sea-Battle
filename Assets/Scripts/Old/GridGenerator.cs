// GridGenerator.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds a grid of GridCellView prefabs under a RectTransform with a GridLayoutGroup.
/// Returns the created GridCellView[,] so other components can update visuals.
/// </summary>
public class GridGenerator : MonoBehaviour
{
    [Header("References")]
    public RectTransform gridParent;        // parent that holds cells
    public GridLayoutGroup layoutGroup;     // grid layout component
    public GridCellView cellPrefab;         // prefab to instantiate

    /// <summary>
    /// Build visual grid and return views arranged [x,y] (x across, y down).
    /// </summary>
    public GridCellView[,] Build(GridModel model, bool isPlayerGrid, System.Action<int, int, bool> onCellClicked)
    {
        ClearParent();

        int cols = model.Width;
        int rows = model.Height;
        var views = new GridCellView[cols, rows];

        // calculate square cell size to fit parent
        Canvas.ForceUpdateCanvases();
        Rect r = gridParent.rect;
        float cellSize = Mathf.Floor(Mathf.Min(r.width / cols, r.height / rows));
        layoutGroup.cellSize = new Vector2(cellSize, cellSize);
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = cols;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                var inst = Instantiate(cellPrefab, gridParent);
                inst.Initialize(x, y, isPlayerGrid, onCellClicked);
                views[x, y] = inst;
            }
        }

        return views;
    }

    private void ClearParent()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--) DestroyImmediate(gridParent.GetChild(i).gameObject);
    }
}
