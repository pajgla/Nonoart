using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ETileState
{
    Unrevealed,
    Completed
}

public enum ETileColor
{
    White,
    Gray
}

public enum ETileDecoration
{
    None,
    RightSeparator,
    TopSeparator,
    TopRightSeparator
}

public class ComplitedTileData
{
    public Color m_Color = Color.white;
    public int m_WidthIndex = 0;
    public int m_HeightIndex = 0;
    public bool m_IsEmpty = true;
}

[RequireComponent(typeof(ExpandedButton))]
public class GridTile : MonoBehaviour
{
    [SerializeField] Image m_BackgroundImageComponent = null;
    [SerializeField] Image m_ForegroundImageComponent = null;

    [SerializeField] Color m_RequiredColor = Color.white;
    ETileState m_TileState = ETileState.Unrevealed;
    bool m_IsColored = false;
    ETileColor m_TileBackgroundColor = ETileColor.White;
    int m_WidthIndex = 0;
    int m_HeightIndex = 0;
    bool m_IsEmpty = false;
    bool m_IsSolved = false;
    bool m_IsMarked = false;

    bool m_IsCursorOver = false;

    private void Update()
    {
        if (m_IsCursorOver)
        {
            GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
            if (Input.GetMouseButton(0)) //Left click
            {
                globalEvents.Invoke_OnTileClicked(this, KeyCode.Mouse0);
            }
            else if (Input.GetMouseButton(1)) //Right click
            {
                globalEvents.Invoke_OnTileClicked(this, KeyCode.Mouse1);
            }
            else if (Input.GetMouseButton(2)) //Middle click
            {
                globalEvents.Invoke_OnTileClicked(this, KeyCode.Mouse2);
            }
        }
    }

    public void OnCursorEnter()
    {
        m_IsCursorOver = true;
    }

    public void OnCursorExit()
    {
        m_IsCursorOver = false;
    }

    public void Init(ComplitedTileData data)
    {
        SetRequiredColor(data.m_Color);
        m_WidthIndex = data.m_WidthIndex;
        m_HeightIndex = data.m_HeightIndex;
        m_TileState = ETileState.Unrevealed;
    }

    // Getters and Setters
    public Color GetRequiredColor() { return m_RequiredColor; }
    public void SetRequiredColor(Color color) 
    {
        m_RequiredColor = color;
        SetIsColored(true);
    }

    public void Paint(Color color)
    {
        m_ForegroundImageComponent.color = color;
        m_ForegroundImageComponent.enabled = true;
        m_IsMarked = false;

        //Used for nonogram creation - we need to notify save system that this tile is not empty
        SetIsEmpty(false);
        SetIsSolved(true);
    }

    public void SetForegroundImage(Sprite sprite)
    {
        GetForegroundImageComponent().sprite = sprite;
        GetForegroundImageComponent().enabled = true;
    }

    public void MarkTile(Sprite sprite)
    {
        SetForegroundImage(sprite);
        m_IsMarked = true;
    }

    public bool GetIsMarked() { return m_IsMarked; }
    public ETileState GetTileState() { return m_TileState; }
    public void SetTileState(ETileState state) { m_TileState = state; }
    public bool GetIsColored() { return m_IsColored; }
    public void SetIsColored(bool isColored) { m_IsColored = isColored; }
    public ETileColor GetTileBackgroundColor() { return m_TileBackgroundColor; }
    public void SetTileBackgroundColor(ETileColor color) { m_TileBackgroundColor = color; }
    public int GetWidthIndex() { return m_WidthIndex; }
    public void SetWidthIndex(int index) { m_WidthIndex = index; }
    public int GetHeightIndex() { return m_HeightIndex; }
    public void SetHeightIndex(int index) { m_HeightIndex = index;}
    public Image GetBackgroundImageComponent() { return m_BackgroundImageComponent; }
    public Image GetForegroundImageComponent() { return m_ForegroundImageComponent; }
    public bool GetIsEmpty() { return m_IsEmpty; }
    public void SetIsEmpty(bool isEmpty) {  m_IsEmpty = isEmpty; }
    public bool GetIsSolved() { return m_IsSolved; }
    public void SetIsSolved(bool isSolved) { m_IsSolved = isSolved; }
}
