using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RectTransform m_GridHolder = null;
    [SerializeField] GridTilesBackgroundDataset m_GridTilesBackgroundDataset = null;

    [Header("Finetuning")]
    [SerializeField] float m_TileSpawnCooldown = 0.1f;

    [SerializeField] GridTile m_TilePrefab = null;


    List<List<GridTile>> m_GridTiles = new List<List<GridTile>>();

    ETileColor m_CurrentTileColor = ETileColor.White;

    public static event EventHandler OnGridSpawnedEvent;

    public IEnumerator SpawnGrid(int gridWidth, int gridHeight)
    {
        RectTransform rectTransform = m_TilePrefab.GetComponent<RectTransform>();

        Vector2 spawnPos = Vector2.zero;
        spawnPos.y = -((rectTransform.sizeDelta.y * gridHeight) / 2 + rectTransform.sizeDelta.y / 2);

        for (int i = 1; i <= gridHeight; i++)
        {
            int currentColumn = i - 1;
            m_GridTiles.Add(new List<GridTile>());

            spawnPos.y += rectTransform.sizeDelta.y;
            spawnPos.x = - ((rectTransform.sizeDelta.x * gridWidth) / 2 - rectTransform.sizeDelta.x / 2);
            if  (i > 1)
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
                newTile.transform.SetParent(m_GridHolder);
                newTile.transform.localScale = Vector3.one;
                newTile.GetComponent<RectTransform>().anchoredPosition = spawnPos;

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
                
                SetTileImage(newTile.gameObject, m_CurrentTileColor, decoration);
                newTile.SetTileBackgroundColor(m_CurrentTileColor);

                ChangeNextTileColor();

                m_GridTiles[currentColumn].Add(newTile);
                spawnPos.x += rectTransform.sizeDelta.x;

                yield return new WaitForSeconds(m_TileSpawnCooldown);
            }
        }

        TriggerOnGridSpawnedEvent();
    }

    private void ChangeNextTileColor()
    {
        if (m_CurrentTileColor == ETileColor.White)
            m_CurrentTileColor = ETileColor.Gray;
        else
            m_CurrentTileColor = ETileColor.White;
    }

    private void SetTileImage(GameObject m_FieldPrefab, ETileColor tileColor, ETileDecoration tileDecoration)
    {
        Sprite spriteToSet = GetAssociatedTileBackground(tileColor, tileDecoration);
        m_FieldPrefab.GetComponent<Image>().GetComponent<Image>().sprite = spriteToSet;
    }

    private Sprite GetAssociatedTileBackground(ETileColor tileColor, ETileDecoration tileDecoration)
    {
        return m_GridTilesBackgroundDataset.GetAssociatedTileSprite(tileColor, tileDecoration);
    }

    void TriggerOnGridSpawnedEvent(EventArgs args = null)
    {
        if (OnGridSpawnedEvent != null)
        {
            OnGridSpawnedEvent(this, args);
        }
        else
        {
            Debug.LogError("OnGridSpawnedEvent is null. Cannot invoke");
        }
    }
}
