using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ShipOrientation { Horizontal, Vertical }


[RequireComponent(typeof(RectTransform), typeof(Image))]
public class Ship : MonoBehaviour
{
    [Header("Ship Settings")]
    public string shipName;
    public int size = 3;
    public ShipOrientation orientation = ShipOrientation.Horizontal;


    [Header("References")]
    public RectTransform rectTransform;
    public Image shipImage;


    [Header("Runtime Data (Filled After Placement)")]
    public List<Cell> occupiedCells = new List<Cell>();
    public Vector2Int placedOrigin;
    public bool isPlaced = false;
    public Vector2 originalSpawnPos; // for undo


    public bool IsPlaced => isPlaced;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        shipImage = GetComponent<Image>();
    }


    public void ClearPlacement()
    {
        isPlaced = false;
        occupiedCells.Clear();
    }
}