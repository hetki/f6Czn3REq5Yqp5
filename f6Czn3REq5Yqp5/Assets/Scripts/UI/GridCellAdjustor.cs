using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCellAdjustor : MonoBehaviour
{
    private int columns = 2;
    private float widthOffset = 20f;
    private float heightOffset = 20f;
    private bool initialized = false;
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;
    private GameBoard gameBoard;
    private bool shouldResetGrid = true;

    /// <summary>
    /// GridCellAdjustor Pre-Init sequence
    /// </summary>
    private void Awake()
    {
        if (!initialized)
            Initialize();
    }

    /// <summary>
    /// GridCellAdjustor Initialization
    /// </summary>
    private void Initialize()
    {
        gameBoard = GetComponent<GameBoard>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        //Set columns
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;

        //Refresh layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        
        initialized = true;
    }

    /// <summary>
    /// On dimensions change
    /// </summary>
    private void OnRectTransformDimensionsChange()
    {
        ResetGridLayout();
    }

    /// <summary>
    /// Adapt grid to new dimensions
    /// </summary>
    public void ResetGridLayout() 
    {
        if (!shouldResetGrid)
            return;

        //Break since we do not want to touch the gridLayout
        if (PlayerPrefs.GetInt("noSave") == -1)
            return;

        if (!initialized)
            Initialize();

        StartCoroutine(ResetGridLayoutRoutine());
    }

    /// <summary>
    /// Adapt grid to new dimensions with controlled delay
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetGridLayoutRoutine()
    {
        bool wasLocked = gameBoard.BoardLocked;
        gameBoard.BoardLocked = true;

        //Wait for UI to update sequence
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = true;
        yield return new WaitForEndOfFrame();
        RefitGridCells();
        yield return MonoHelper.GetWait(0.25f);
        gridLayoutGroup.enabled = false;

        if(!wasLocked)
            gameBoard.BoardLocked = false;
    }

    /// <summary>
    /// Refit grid cells to grid
    /// </summary>
    void RefitGridCells()
    {
        if (!initialized)
            Initialize();

        Vector2 cellSize = CalculateCellSize();
        gridLayoutGroup.cellSize = cellSize;
    }

    /// <summary>
    /// Calculate optimal grid cell size based on card layout and rect dimensions
    /// </summary>
    /// <returns>Vector2</returns>
    private Vector2 CalculateCellSize()
    {
        CVector2 selectedLayout = MonoHelper.StringToCardLayout(PlayerPrefs.GetString("cardLayout"));

        float cellWidth = (rectTransform.rect.width / selectedLayout.x) - widthOffset;
        float cellHeight = (rectTransform.rect.height / selectedLayout.y) - heightOffset;

        //Use smallest "best" fit to ensure it's always a square
        float smallestSize = (cellWidth < cellHeight) ? cellWidth : cellHeight;

        return new Vector2(smallestSize, smallestSize);
    }

    private void OnDisable()
    {
        //Prevent grid reset after disabled
        shouldResetGrid = false;
    }
}
