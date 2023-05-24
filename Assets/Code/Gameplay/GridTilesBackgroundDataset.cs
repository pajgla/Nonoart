using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct GridTileBackgroundPair
{
    public ETileColor color;
    public ETileDecoration tileDecoration;
    public Sprite tileSprite;
}

[CreateAssetMenu(fileName = "Grid Tile Background Dataset", menuName = "Tiles/Background Dataset")]
public class GridTilesBackgroundDataset : ScriptableObject
{
    [SerializeField]
    List<GridTileBackgroundPair> m_GridTileBackgroundPairs = new List<GridTileBackgroundPair>();

    public Sprite GetAssociatedTileSprite(ETileColor color, ETileDecoration tileDecoration)
    {
        foreach (GridTileBackgroundPair pair in m_GridTileBackgroundPairs)
        {
            if (pair.color == color && pair.tileDecoration == tileDecoration)
                return pair.tileSprite;
        }

        Debug.LogError("Could not find associated sprite for: " + color.ToString() + " color and " + tileDecoration.ToString() + " decoration.");
        return null;
    }
}
