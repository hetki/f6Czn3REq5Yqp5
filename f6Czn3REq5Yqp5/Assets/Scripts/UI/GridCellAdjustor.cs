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

    private void Awake()
    {
        if (!initialized)
            Initialize();
    }

    private void Initialize()
    {
        gameBoard = GetComponent<GameBoard>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        
        initialized = true;
    }

    private void OnRectTransformDimensionsChange()
    {
        ResetGridLayout();
    }

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

    IEnumerator ResetGridLayoutRoutine()
    {
        MonoHelper.Log("RESET GRID");
        MonoHelper.Log("Graceful Exit?: " + PlayerPrefs.GetInt("noSave"));

        bool wasLocked = gameBoard.BoardLocked;
        gameBoard.BoardLocked = true;

        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = true;
        yield return new WaitForEndOfFrame();
        RefitGridCells();
        yield return MonoHelper.GetWait(0.25f);
        gridLayoutGroup.enabled = false;

        if(!wasLocked)
            gameBoard.BoardLocked = false;
    }

    void RefitGridCells()
    {
        if (!initialized)
            Initialize();

        Vector2 cellSize = CalculateCellSize();
        gridLayoutGroup.cellSize = cellSize;
    }

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
        shouldResetGrid = false;
    }
}
