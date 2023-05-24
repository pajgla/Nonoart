using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nonogram : ScriptableObject
{
    List<List<GridTile>> m_GridTiles = new List<List<GridTile>>();

    public void Initialize(int width, int height)
    {
        m_GridTiles.Clear();

        for (int i = 0; i < height; i++)
        {
            m_GridTiles.Add(new List<GridTile>());

            for (int j = 0; j < width; j++)
            {
                m_GridTiles[i].Add(new GridTile());
            }
        }
    }
}
