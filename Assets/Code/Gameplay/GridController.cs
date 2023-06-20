using System;
using System.Collections;
using System.Collections.Generic;
using UIViewModel;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GridController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GridViewModel m_GridViewModelRef = null;
    [SerializeField] GridTilesBackgroundDataset m_GridTilesBackgroundDataset = null;
    [SerializeField] PixelCountWidget m_PixelCountWidget = null;
    [SerializeField] GridTile m_TilePrefab = null;

    GridViewModel m_GridViewModel = null;

    [Header("Finetuning")]
    [SerializeField] float m_TileSpawnCooldown = 0.1f;

    List<List<GridTile>> m_GridTiles = new List<List<GridTile>>();
    List<PixelCountWidget> m_HorizontalPixelCountWidgets = new List<PixelCountWidget>();
    List<PixelCountWidget> m_VerticalPixelCountWidgets = new List<PixelCountWidget>();

    ETileBackgroundColor m_CurrentTileColor = ETileBackgroundColor.White;

    int m_GridWidth = 0;
    int m_GridHeight = 0;

    int m_TotalTilesToSolve = 0;

    public void Init()
    {
        m_GridViewModel = ViewModelHelper.SpawnAndInitialize(m_GridViewModelRef) as GridViewModel;
        GameManager.Get().GetGlobalEvents().e_OnTilePainted += OnTilePainted;
    }

    public IEnumerator SpawnGrid(Nonogram nonogram)
    {
        m_GridWidth = nonogram.GetWidth();
        m_GridHeight = nonogram.GetHeight();

        RectTransform gridHolder = m_GridViewModel.GetGridHolder();

        RectTransform rectTransform = m_TilePrefab.GetComponent<RectTransform>();

        Vector2 spawnPos = Vector2.zero;
        spawnPos.y = -((rectTransform.sizeDelta.y * m_GridHeight) / 2 + rectTransform.sizeDelta.y / 2);

        for (int height = 1; height <= m_GridHeight; height++)
        {
            int currentColumn = height - 1;
            m_GridTiles.Add(new List<GridTile>());

            spawnPos.y += rectTransform.sizeDelta.y;
            spawnPos.x = -((rectTransform.sizeDelta.x * m_GridWidth) / 2 - rectTransform.sizeDelta.x / 2);
            if (height > 1)
            {
                int columnToCheck = height - 2;
                int rowToCheck = 0;

                if (m_GridTiles[columnToCheck][rowToCheck].GetTileBackgroundColor() == m_CurrentTileColor)
                {
                    ChangeNextTileColor();
                }
            }

            for (int width = 1; width <= m_GridWidth; ++width)
            {
                GridTile newTile = Instantiate(m_TilePrefab, Vector3.zero, Quaternion.identity);
                newTile.transform.SetParent(gridHolder);
                newTile.transform.localScale = Vector3.one;
                newTile.GetComponent<RectTransform>().anchoredPosition = spawnPos;
                newTile.SetWidthIndex(width);
                newTile.SetHeightIndex(height);

                ETileDecoration decoration = ETileDecoration.None;
                if (height % 5 == 0 && width % 5 == 0)
                {
                    decoration = ETileDecoration.TopRightSeparator;
                }
                else if (width % 5 == 0)
                {
                    decoration = ETileDecoration.RightSeparator;
                }
                else if (height % 5 == 0)
                {
                    decoration = ETileDecoration.TopSeparator;
                }

                SetTileImage(newTile, m_CurrentTileColor, decoration);
                newTile.SetTileBackgroundColor(m_CurrentTileColor);

                ChangeNextTileColor();

                m_GridTiles[currentColumn].Add(newTile);
                spawnPos.x += rectTransform.sizeDelta.x;

                yield return new WaitForSeconds(m_TileSpawnCooldown);
            }
        }

        foreach (ComplitedTileData tileData in nonogram.GetComplitedTilesData())
        {
            GridTile tile = m_GridTiles[tileData.m_HeightIndex - 1][tileData.m_WidthIndex - 1];
            tile.SetRequiredColor(tileData.m_Color);
            m_TotalTilesToSolve++;
        }

        SpawnPixelCountWidgets();
        TriggerOnGridSpawnedEvent();
    }

    public IEnumerator SpawnEmptyGrid(int gridWidth, int gridHeight)
    {
        m_GridWidth = gridWidth;
        m_GridHeight = gridHeight;

        RectTransform gridHolder = m_GridViewModel.GetGridHolder();

        RectTransform rectTransform = m_TilePrefab.GetComponent<RectTransform>();

        Vector2 spawnPos = Vector2.zero;
        spawnPos.y = -((rectTransform.sizeDelta.y * gridHeight) / 2 + rectTransform.sizeDelta.y / 2);

        for (int height = 1; height <= gridHeight; height++)
        {
            int currentColumn = height - 1;
            m_GridTiles.Add(new List<GridTile>());

            spawnPos.y += rectTransform.sizeDelta.y;
            spawnPos.x = -((rectTransform.sizeDelta.x * gridWidth) / 2 - rectTransform.sizeDelta.x / 2);
            if (height > 1)
            {
                int columnToCheck = height - 2;
                int rowToCheck = 0;

                if (m_GridTiles[columnToCheck][rowToCheck].GetTileBackgroundColor() == m_CurrentTileColor)
                {
                    ChangeNextTileColor();
                }
            }

            for (int width = 1; width <= gridWidth; ++width)
            {
                GridTile newTile = Instantiate(m_TilePrefab, Vector3.zero, Quaternion.identity);
                newTile.transform.SetParent(gridHolder);
                newTile.transform.localScale = Vector3.one;
                newTile.GetComponent<RectTransform>().anchoredPosition = spawnPos;
                newTile.SetWidthIndex(width);
                newTile.SetHeightIndex(height);

                ETileDecoration decoration = ETileDecoration.None;
                if (height % 5 == 0 && width % 5 == 0)
                {
                    decoration = ETileDecoration.TopRightSeparator;
                }
                else if (width % 5 == 0)
                {
                    decoration = ETileDecoration.RightSeparator;
                }
                else if (height % 5 == 0)
                {
                    decoration = ETileDecoration.TopSeparator;
                }

                SetTileImage(newTile, m_CurrentTileColor, decoration);
                newTile.SetTileBackgroundColor(m_CurrentTileColor);

                ChangeNextTileColor();

                m_GridTiles[currentColumn].Add(newTile);
                spawnPos.x += rectTransform.sizeDelta.x;

                yield return new WaitForSeconds(m_TileSpawnCooldown);
            }
        }

        SpawnPixelCountWidgets();
        TriggerOnGridSpawnedEvent();
    }

    private void PopulatePixelCounts(PixelCountWidget widget, int width, int height, bool isVertical)
    {
        int counter = 0;
        bool isCounting = false;

        int start = isVertical ? m_GridHeight - 1 : 0;
        int end = isVertical ? -1 : m_GridWidth;
        int increment = isVertical ? -1 : 1;

        for (int i = start; i != end; i += increment)
        {
            GridTile currTile = isVertical ? m_GridTiles[i][width] : m_GridTiles[height][i];

            if (currTile.GetIsColored())
            {
                if (isCounting)
                {
                    counter++;
                }
                else
                {
                    isCounting = true;
                    counter++;
                }
            }
            else
            {
                if (isCounting)
                {
                    isCounting = false;
                    widget.AddClue(counter);
                    counter = 0;
                }
            }
        }

        if (isCounting)
        {
            widget.AddClue(counter);
        }
    }

    private void SpawnPixelCountWidgetsPerAxis(bool isVertical)
    {
        int end = isVertical ? m_GridHeight : m_GridWidth;
        for (int i = 0; i < end; i++)
        {
            PixelCountWidget pixelCountWidget = Instantiate(m_PixelCountWidget);
            pixelCountWidget.SetIsVertical(isVertical);
            //Get the first tile to set the position relative to it
            GridTile tile = isVertical ? m_GridTiles[m_GridHeight - 1][i] : m_GridTiles[i][0];
            RectTransform tileRectTransform = tile.GetComponent<RectTransform>();
            pixelCountWidget.AdjustPositionRelativeTo(tileRectTransform);
            int width = isVertical ? i : 0;
            int height = isVertical ? m_GridHeight - 1 : i;
            PopulatePixelCounts(pixelCountWidget, width, height, isVertical);
            if (isVertical)
            {
                m_VerticalPixelCountWidgets.Add(pixelCountWidget);
            }
            else
            {
                m_HorizontalPixelCountWidgets.Add(pixelCountWidget);
            }
        }
    }

    private void SpawnPixelCountWidgets()
    {
        SpawnPixelCountWidgetsPerAxis(false);
        SpawnPixelCountWidgetsPerAxis(true);
    }

    private void ChangeNextTileColor()
    {
        if (m_CurrentTileColor == ETileBackgroundColor.White)
            m_CurrentTileColor = ETileBackgroundColor.Gray;
        else
            m_CurrentTileColor = ETileBackgroundColor.White;
    }

    private void SetTileImage(GridTile m_FieldPrefab, ETileBackgroundColor tileColor, ETileDecoration tileDecoration)
    {
        Sprite spriteToSet = GetAssociatedTileBackground(tileColor, tileDecoration);
        m_FieldPrefab.GetBackgroundImageComponent().sprite = spriteToSet;
        m_FieldPrefab.GetForegroundImageComponent().enabled = false;
    }

    private Sprite GetAssociatedTileBackground(ETileBackgroundColor tileColor, ETileDecoration tileDecoration)
    {
        return m_GridTilesBackgroundDataset.GetAssociatedTileSprite(tileColor, tileDecoration);
    }

    void TriggerOnGridSpawnedEvent()
    {
        GameManager.Get().GetGlobalEvents().Invoke_OnGridSpawned();
    }

    public RectTransform GetGridHolder()
    {
        return m_GridViewModel.GetGridHolder();
    }

    public Nonogram CreateNonogram()
    {
        Nonogram newNonogram = new Nonogram();
        newNonogram.CreateNonogram(m_GridTiles);
        newNonogram.SetWidth(m_GridWidth);
        newNonogram.SetHeight(m_GridHeight);
        return newNonogram;
    }

    public void OnTilePainted(GridTile tile)
    {
        int row = tile.GetWidthIndex() - 1;
        int column = tile.GetHeightIndex() - 1;
        RefreshPixelCountWidget(row, column, false);
        RefreshPixelCountWidget(row, column, true);
    }

    public void DeleteCluesFromWidget(int row, int column, bool isVertical)
    {
        PixelCountWidget pixelCountWidget = isVertical ? m_VerticalPixelCountWidgets[row] : m_HorizontalPixelCountWidgets[column];
        pixelCountWidget.DeleteClues();
    }

    public void RefreshPixelCountWidget(int row, int column, bool isVertical, bool forceMarkAsSolved = false)
    {
        int tilesCounter = 0;
        int widgetCounter = 0;
        bool isCountingTiles = false;
        //Easier to work with true as default
        bool shouldMarkAsSolved = true;

        int end = isVertical ? m_GridHeight : m_GridWidth;
        for (int i = 0; i < end; i++)
        {
            GridTile currentTile = isVertical ? m_GridTiles[i][row] : m_GridTiles[column][i];
            if (currentTile.GetIsColored())
            {
                if (!isCountingTiles)
                    isCountingTiles = true;

                if (!currentTile.GetIsSolved() && !forceMarkAsSolved)
                {
                    shouldMarkAsSolved = false;
                }

                tilesCounter++;
            }
            else
            {
                if (isCountingTiles)
                {
                    if (shouldMarkAsSolved)
                    {
                        PixelCountWidget pixelCountWidget = isVertical ? m_VerticalPixelCountWidgets[row] : m_HorizontalPixelCountWidgets[column];
                        SolveOrAddClue(pixelCountWidget, widgetCounter, tilesCounter);
                    }

                    widgetCounter++;
                    //Reset to default values
                    shouldMarkAsSolved = true;
                    isCountingTiles = false;
                    tilesCounter = 0;
                }
            }
        }

        if (shouldMarkAsSolved && isCountingTiles)
        {
            PixelCountWidget pixelCountWidget = isVertical ? m_VerticalPixelCountWidgets[row] : m_HorizontalPixelCountWidgets[column];
            SolveOrAddClue(pixelCountWidget, widgetCounter, tilesCounter);
        }
    }

    private void SolveOrAddClue(PixelCountWidget widget, int clueIndex, int value)
    {
        if (widget.IsClueHidden(clueIndex))
        {
            widget.SetClueValue(clueIndex, value);
        }
        else if (widget.DoesClueExist(clueIndex))
        {
            widget.SetClueSolved(clueIndex);
        }
        else
        {
            widget.AddClue(value);
        }
    }

    public int GetTotalTilesToSolve()
    {
        return m_TotalTilesToSolve;
    }
}
