using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Nonogram
{
    private string m_NonogramName = "New Nonogram";

    List<List<GridTile>> m_GridTiles = new List<List<GridTile>>();
    List<List<ComplitedTileData>> m_ComplitionTiles = new List<List<ComplitedTileData>>();
    int m_Width = 0;
    int m_Height = 0;

    public void Initialize(int width, int height)
    {
        m_GridTiles.Clear();

        for (int i = 0; i < height; i++)
        {
            m_GridTiles.Add(new List<GridTile>());

            for (int j = 0; j < width; j++)
            {
                GridTile newTile = new GridTile();
                newTile.SetIsEmpty(true);
                m_GridTiles[i].Add(newTile);
            }
        }
    }

    public void CreateNonogram(List<List<GridTile>> gridTiles)
    {
        m_GridTiles = new List<List<GridTile>>(gridTiles);
    }

    public void AddRequiredColors(List<List<ComplitedTileData>> data)
    {
        m_ComplitionTiles = new List<List<ComplitedTileData>>(data);
    }

    public void SetNonogramName(string name)
    {
        m_NonogramName = name;
    }

    public string GetNonogramName()
    {
        return m_NonogramName;
    }

    public List<List<GridTile>> GetTiles()
    {
        return m_GridTiles;
    }

    public Texture2D ConvertToTexture()
    {
        Texture2D newTexture = new Texture2D(m_Width, m_Height, TextureFormat.ARGB32, false);

        newTexture.filterMode = FilterMode.Point;
        for (int height = 0; height < m_Height; ++height)
        {
            for (int width = 0; width < m_Width; ++width)
            {
                ComplitedTileData complitedTileData = m_ComplitionTiles[height][width];
                Color colorToUse = Color.white;
                if (complitedTileData.m_IsEmpty)
                {
                    colorToUse = Color.white;
                    colorToUse.a = 0.0f;
                }
                else
                {
                    colorToUse = complitedTileData.m_Color;
                }

                newTexture.SetPixel(width, height, colorToUse);
            }
        }

        newTexture.Apply();
        File.WriteAllBytes(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Texture.jpg", newTexture.EncodeToJPG(1));
        return newTexture;
    }

    public int GetWidth() { return m_Width; }
    public int GetHeight() { return m_Height; }
    public void SetWidth(int width) {  m_Width = width; }
    public void SetHeight(int height) {  m_Height = height; }
    public List<List<ComplitedTileData>> GetComplitedTilesData() { return m_ComplitionTiles;}
}

[Serializable]
public class NonogramSet
{
    string m_Name = string.Empty;
    List<Nonogram> m_Nonograms = new List<Nonogram>();

    public void SetName(string name)
    {
        m_Name = name;
    }

    public string GetName() { return m_Name; }
    public List<Nonogram> GetNonograms() { return m_Nonograms; }
}

public class NonogramSaveData
{
    [Serializable]
    public struct TileColorSaveData
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public bool IsEmpty;
    }

    //Members are saved as json so keep names simple and without prefixes
    [SerializeField] List<TileColorSaveData> TileSaveData = new List<TileColorSaveData>();
    [SerializeField] int Width = 0;
    [SerializeField] int Height = 0;

    public void Init(Nonogram nonogram)
    {
        List<List<GridTile>> allTiles = nonogram.GetTiles();
        if (allTiles.Count == 0)
        {
            Debug.LogError("Empty nonogram provided");
            return;
        }

        foreach (List<GridTile> row in allTiles)
        {
            foreach (GridTile tile in row)
            {
                Color tileColor = tile.GetRequiredColor();
                TileColorSaveData data = new TileColorSaveData();
                data.r = tileColor.r;
                data.g = tileColor.g;
                data.b = tileColor.b;
                data.a = tileColor.a;
                data.IsEmpty = tile.GetIsEmpty();

                TileSaveData.Add(data);
            }
        }

        Width = nonogram.GetWidth();
        Height = nonogram.GetHeight();
    }

    public Nonogram ConvertToNonogram()
    {
        if ((Width * Height == TileSaveData.Count) == false)
        {
            Debug.LogError("Saved Data is corrupted or invalid.");
            return null;
        }

        Nonogram newNonogram = new Nonogram();
        newNonogram.SetWidth(Width);
        newNonogram.SetHeight(Height);

        List<List<ComplitedTileData>> gridTiles = new List<List<ComplitedTileData>>();
        int tileDataIndex = 0;
        for (int currHeight = 0; currHeight < Height; currHeight++)
        {
            List<ComplitedTileData> tiles = new List<ComplitedTileData>();
            for (int currWidth = 0; currWidth < Width; currWidth++)
            {
                ComplitedTileData tile = new ComplitedTileData();
                tile.m_HeightIndex = currHeight;
                tile.m_WidthIndex = currWidth;

                TileColorSaveData tileData = TileSaveData[tileDataIndex];
                Color newColor = new Color(tileData.r, tileData.g, tileData.b, tileData.a);
                tile.m_Color = newColor;
                tile.m_IsEmpty = tileData.IsEmpty;
                tiles.Add(tile);
                tileDataIndex++;
            }

            gridTiles.Add(tiles);
        }

        newNonogram.AddRequiredColors(gridTiles);

        return newNonogram;
    }

    public List<TileColorSaveData> GetTileSaveData() { return TileSaveData; }
    public int GetWidth() { return Width; }
    public int GetHeight() { return Height; }
}
