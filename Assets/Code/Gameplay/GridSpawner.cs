using System;
using System.Collections;
using System.Collections.Generic;
using UIViewModel;
using UnityEngine;
using UnityEngine.UI;

public class GridSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GridViewModel m_GridViewModelRef = null;
    [SerializeField] GridTilesBackgroundDataset m_GridTilesBackgroundDataset = null;
    [SerializeField] PixelCountWidget m_PixelCountWidget = null;

    GridViewModel m_GridViewModel = null;

    [Header("Finetuning")]
    [SerializeField] float m_TileSpawnCooldown = 0.1f;

    [SerializeField] GridTile m_TilePrefab = null;


    List<List<GridTile>> m_GridTiles = new List<List<GridTile>>();

    ETileColor m_CurrentTileColor = ETileColor.White;

    int m_GridWidth = 0;
    int m_GridHeight = 0;

    public static event EventHandler OnGridSpawnedEvent;
    public static event EventHandler OnTileSpawnedEvent;

    public void Init()
    {
        m_GridViewModel = ViewModelHelper.SpawnAndInitialize(m_GridViewModelRef) as GridViewModel;
    }

    public IEnumerator SpawnGrid(int gridWidth, int gridHeight)
    {
        m_GridWidth = gridWidth;
        m_GridHeight = gridHeight;

        RectTransform gridHolder = m_GridViewModel.GetGridHolder();

        RectTransform rectTransform = m_TilePrefab.GetComponent<RectTransform>();

        Vector2 spawnPos = Vector2.zero;
        spawnPos.y = -((rectTransform.sizeDelta.y * gridHeight) / 2 + rectTransform.sizeDelta.y / 2);

        for (int i = 1; i <= gridHeight; i++)
        {
            int currentColumn = i - 1;
            m_GridTiles.Add(new List<GridTile>());

            spawnPos.y += rectTransform.sizeDelta.y;
            spawnPos.x = -((rectTransform.sizeDelta.x * gridWidth) / 2 - rectTransform.sizeDelta.x / 2);
            if (i > 1)
            {
                int columnToCheck = i - 2;
                int rowToCheck = 0;

                if (m_GridTiles[columnToCheck][rowToCheck].GetTileBackgroundColor() == m_CurrentTileColor)
                {
                    ChangeNextTileColor();
                }
            }

            for (int j = 1; j <= gridWidth; ++j)
            {
                GridTile newTile = Instantiate(m_TilePrefab, Vector3.zero, Quaternion.identity);
                newTile.transform.SetParent(gridHolder);
                newTile.transform.localScale = Vector3.one;
                newTile.GetComponent<RectTransform>().anchoredPosition = spawnPos;
                newTile.SetWidthIndex(j);
                newTile.SetHeightIndex(i);

                ETileDecoration decoration = ETileDecoration.None;
                if (i % 5 == 0 && j % 5 == 0)
                {
                    decoration = ETileDecoration.TopRightSeparator;
                }
                else if (j % 5 == 0)
                {
                    decoration = ETileDecoration.RightSeparator;
                }
                else if (i % 5 == 0)
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

    private void SpawnPixelCountWidgets()
    {
        for (int height = 0; height < m_GridHeight; height++)
        {
            PixelCountWidget pixelCountWidget = Instantiate(m_PixelCountWidget);
            GridTile tile = m_GridTiles[height][0];
            pixelCountWidget.AdjustPositionRelativeTo(tile.GetComponent<RectTransform>());

            //Count pixels
            int counter = 0;
            bool isCounting = false;
            for (int i = 0; i < m_GridWidth; i++)
            {
                GridTile currTile = m_GridTiles[height][i];
                if (!currTile.GetIsEmpty())
                {
                    if (isCounting)
                        counter++;
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
                        pixelCountWidget.AddPixelCount(counter);
                        counter = 0;
                    }
                }
            }
        }

        for (int width = 0; width < m_GridHeight; width++)
        {
            PixelCountWidget pixelCountWidget = Instantiate(m_PixelCountWidget);
            pixelCountWidget.SetIsVertical(true);
            GridTile tile = m_GridTiles[m_GridHeight - 1][width];
            pixelCountWidget.AdjustPositionRelativeTo(tile.GetComponent<RectTransform>());

            //Count pixels
            int counter = 0;
            bool isCounting = false;
            for (int i = 0; i < m_GridWidth; i++)
            {
                GridTile currTile = m_GridTiles[m_GridHeight - 1][i];
                if (!currTile.GetIsEmpty())
                {
                    if (isCounting)
                        counter++;
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
                        pixelCountWidget.AddPixelCount(counter);
                        counter = 0;
                    }
                }
            }
        }
    }

    private void ChangeNextTileColor()
    {
        if (m_CurrentTileColor == ETileColor.White)
            m_CurrentTileColor = ETileColor.Gray;
        else
            m_CurrentTileColor = ETileColor.White;
    }

    private void SetTileImage(GridTile m_FieldPrefab, ETileColor tileColor, ETileDecoration tileDecoration)
    {
        Sprite spriteToSet = GetAssociatedTileBackground(tileColor, tileDecoration);
        m_FieldPrefab.GetBackgroundImageComponent().sprite = spriteToSet;
        m_FieldPrefab.GetForegroundImageComponent().enabled = false;
    }

    private Sprite GetAssociatedTileBackground(ETileColor tileColor, ETileDecoration tileDecoration)
    {
        return m_GridTilesBackgroundDataset.GetAssociatedTileSprite(tileColor, tileDecoration);
    }

    void TriggerOnGridSpawnedEvent(EventArgs args = null)
    {
        OnGridSpawnedEvent?.Invoke(this, args);
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
}
