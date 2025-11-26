using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper that computes the local position of the center of a cell inside a GridLayoutGroup-backed parent.
/// The returned position is local to the gridParent RectTransform (use RectTransform.localPosition or Transform.localPosition).
/// </summary>
public static class GridUIHelper
{
    /// <summary>
    /// Get the local position inside gridParent RectTransform for the center of cell (x,y).
    /// Assumes GridLayoutGroup lays out left->right, top->down order, with (0,0) at top-left.
    /// </summary>
    public static Vector3 GetCellLocalPosition(Transform gridParent, int x, int y)
    {
        var gridRT = gridParent as RectTransform;
        var glg = gridParent.GetComponent<GridLayoutGroup>();
        if (gridRT == null || glg == null)
        {
            Debug.LogWarning("GridUIHelper: gridParent must have RectTransform and GridLayoutGroup.");
            return Vector3.zero;
        }

        // rect sizes
        Rect rect = gridRT.rect;

        // compute starting top-left center
        float cellW = glg.cellSize.x;
        float cellH = glg.cellSize.y;
        float spacingX = glg.spacing.x;
        float spacingY = glg.spacing.y;
        var pad = glg.padding;

        // left-most center X
        float startX = -rect.width * 0.5f + pad.left + cellW * 0.5f;
        // top-most center Y
        float startY = rect.height * 0.5f - pad.top - cellH * 0.5f;

        float posX = startX + x * (cellW + spacingX);
        float posY = startY - y * (cellH + spacingY);

        return new Vector3(posX, posY, 0f);
    }
}
