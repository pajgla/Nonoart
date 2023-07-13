using Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Nonogram
{
    private string m_NonogramName = "New Nonogram";
    private string m_NonogramCategory = "Invalid";
    List<ComplitedTileData> m_ComplitionTiles = new List<ComplitedTileData>();
    int m_Width = 0;
    int m_Height = 0;
    string m_ID = string.Empty;

    public void CreateNonogram(List<List<GridTile>> gridTiles)
    {
        foreach (List<GridTile> gridTileList in gridTiles)
        {
            foreach (GridTile tile in gridTileList)
            {
                if (tile.GetIsColored())
                {
                    ComplitedTileData newTileData = new ComplitedTileData(tile);
                    m_ComplitionTiles.Add(newTileData);
                }
            }
        }
    }

    public void AddRequiredColors(List<ComplitedTileData> data)
    {
        m_ComplitionTiles = new List<ComplitedTileData>(data);
    }

    public void SetNonogramName(string name)
    {
        m_NonogramName = name;
    }

    public string GetNonogramName()
    {
        return m_NonogramName;
    }

    public void SetNonogramID(string id)
    {
        m_ID = id;
    }

    public string GetNonogramID() { return m_ID; }

    public Texture2D GetAsTexture()
    {
        Texture2D newTexture = new Texture2D(m_Width, m_Height, TextureFormat.ARGB32, false);

        newTexture.filterMode = FilterMode.Point;
        for (int height = 0; height < m_Height; ++height)
        {
            for (int width = 0; width < m_Width; ++width)
            {
                //Set background alpha for each pixel
                Color colorToUse = Color.white;
                colorToUse = Color.white;
                colorToUse.a = 0.0f;
                newTexture.SetPixel(width, height, colorToUse);
            }
        }

        foreach (ComplitedTileData tileData in m_ComplitionTiles)
        {
            newTexture.SetPixel(tileData.m_WidthIndex - 1, tileData.m_HeightIndex - 1, tileData.m_Color);
        }

        newTexture.wrapMode = TextureWrapMode.Clamp;

        newTexture.Apply();
        return newTexture;
    }

    public int GetWidth() { return m_Width; }
    public int GetHeight() { return m_Height; }
    public void SetWidth(int width) {  m_Width = width; }
    public void SetHeight(int height) {  m_Height = height; }
    public List<ComplitedTileData> GetComplitedTilesData() { return m_ComplitionTiles;}
    public string GetNonogramCategory() { return m_NonogramCategory; }
    public void SetNonogramCategory(string value) { m_NonogramCategory = value; }
}

public class ComplitedTileData
{
    public Color m_Color = Color.white;
    public int m_WidthIndex = 0;
    public int m_HeightIndex = 0;

    public ComplitedTileData(GridTile tile)
    {
        if (tile.GetIsColored() == false)
        {
            Debug.LogError("Non-colored tile provided to Complited Tile Data");
            return;
        }

        m_Color = tile.GetRequiredColor();
        m_WidthIndex = tile.GetWidthIndex();
        m_HeightIndex = tile.GetHeightIndex();
    }

    public ComplitedTileData() { }
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

    public void AddNonogram(Nonogram nonogram)
    {
        nonogram.SetNonogramCategory(m_Name);
        GetNonograms().Add(nonogram);
    }
}

public class NonogramSaveData
{
    [Serializable]
    public struct TileSaveData
    {
        //Colors
        public float r;
        public float g;
        public float b;
        public float a;
        public int widthIndex;
        public int heightIndex;
    }

    [SerializeField] List<TileSaveData> m_TileSaveDataList = new List<TileSaveData>();
    [SerializeField] int m_Width = 0;
    [SerializeField] int m_Height = 0;
    [SerializeField] string m_ID = string.Empty; 
    //If you change how nonograms are saved or loaded, update the save version accordingly
    //Note that changing the Save Version will make all previous unsuported nonograms deprecated
    [SerializeField] int m_SaveVersion = 1;

    public void Init(Nonogram nonogram)
    {
        foreach (ComplitedTileData tileData in nonogram.GetComplitedTilesData())
        {
            Color tileColor = tileData.m_Color;
            TileSaveData saveData = new TileSaveData();
            saveData.r = tileColor.r;
            saveData.g = tileColor.g;
            saveData.b = tileColor.b;
            saveData.a = tileColor.a;
            saveData.widthIndex = tileData.m_WidthIndex;
            saveData.heightIndex = tileData.m_HeightIndex;

            m_TileSaveDataList.Add(saveData);
        }

        m_Width = nonogram.GetWidth();
        m_Height = nonogram.GetHeight();

        m_ID = nonogram.GetNonogramID();
        if (m_ID == string.Empty)
        {
            Debug.LogError("Nonogram has invalid ID. Generating a new random ID.");
            //#HACK: it is hacky but it works
            string newID = SavegameManager.GenerateUniqueID();
            m_ID = newID;
            nonogram.SetNonogramID(newID);
        }
    }

    public Nonogram ConvertToNonogram()
    {
        Nonogram newNonogram = new Nonogram();
        newNonogram.SetWidth(m_Width);
        newNonogram.SetHeight(m_Height);
        Debug.Assert(m_ID != string.Empty, "Nonogram Save Data has invalid ID");
        newNonogram.SetNonogramID(m_ID);

        List<ComplitedTileData> gridTiles = new List<ComplitedTileData>();
        foreach (TileSaveData tileSaveData in m_TileSaveDataList)
        {
            ComplitedTileData newComplitedTileData = new ComplitedTileData();
            newComplitedTileData.m_WidthIndex = tileSaveData.widthIndex;
            newComplitedTileData.m_HeightIndex = tileSaveData.heightIndex;

            Color newColor = new Color(tileSaveData.r, tileSaveData.g, tileSaveData.b, tileSaveData.a);
            newComplitedTileData.m_Color = newColor;

            gridTiles.Add(newComplitedTileData);
        }

        newNonogram.AddRequiredColors(gridTiles);

        return newNonogram;
    }

    public List<TileSaveData> GetTileSaveData() { return m_TileSaveDataList; }
    public int GetWidth() { return m_Width; }
    public int GetHeight() { return m_Height; }
}
