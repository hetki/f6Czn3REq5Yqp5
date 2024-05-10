using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCellAdjustor : MonoBehaviour
{
    [SerializeField]
    private int columns = 2;
    [SerializeField]
    private float widthOffset = 20f;
    [SerializeField]
    private float heightOffset = 20f;

    private bool initialized = false;
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    private void Awake()
    {
        if (!initialized)
            Initialize();
    }

    private void Initialize()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        
        initialized = true;
    }

    private void OnRectTransformDimensionsChange()
    {
        RefitGridCells();
    }

    public void RefitGridCells()
    {
        if (!initialized)
            Initialize();

        Vector2 cellSize = CalculateCellSize();
        gridLayout.cellSize = cellSize;
    }

    private Vector2 CalculateCellSize()
    {
        int childCount = transform.childCount;
        
        int rowCount = (int)Mathf.Ceil((float)childCount / columns);

        float cellWidth = (rectTransform.rect.width / columns) - widthOffset;
        float cellHeight = (rectTransform.rect.height / rowCount) - heightOffset;

        //Use smallest "best" fit to ensure it's always a square
        float smallestSize = (cellWidth < cellHeight) ? cellWidth : cellHeight;

        return new Vector2(smallestSize, smallestSize);
    }
}
