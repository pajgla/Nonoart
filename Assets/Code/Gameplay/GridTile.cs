using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETileState
{
    Empty,
    Marked,
    Crossed,
    Completed,
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

public class GridTile : MonoBehaviour
{
    Color m_RequiredColor = Color.white;
    ETileState m_TileState = ETileState.Empty;
    bool m_IsColored = false;
    ETileColor m_TileBackgroundColor = ETileColor.White;

    public Color GetRequiredColor() { return m_RequiredColor; }
    public void SetRequiredColor(Color color) { m_RequiredColor = color; }
    public ETileState GetTileState() { return m_TileState; }
    public void SetTileState(ETileState state) { m_TileState = state; }
    public bool GetIsColored() { return m_IsColored; }
    public void SetIsColored(bool isColored) { m_IsColored = isColored; }
    public ETileColor GetTileBackgroundColor() { return m_TileBackgroundColor; }
    public void SetTileBackgroundColor(ETileColor color) { m_TileBackgroundColor = color; }
}
