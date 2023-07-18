using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingController
{
    public enum EDrawingType
    {
        Free,
        Line
    }

    EDrawingType m_ActiveDrawingMode = EDrawingType.Free;
    bool m_IsLineDrawingInProgress = false;
    Vector2 m_StartingLineDrawingTilePosition = Vector2.zero;
    Vector2 m_LineDrawingDirection = Vector2.zero;

    public EDrawingType ToggleDrawingMode()
    {
        if (m_IsLineDrawingInProgress) 
            return m_ActiveDrawingMode;

        if (m_ActiveDrawingMode == EDrawingType.Line)
            m_ActiveDrawingMode = EDrawingType.Free;
        else
            m_ActiveDrawingMode = EDrawingType.Line;

        ResetLineDrawing();

        return m_ActiveDrawingMode;
    }

    public EDrawingType GetActiveDrawingMode()
    {
        return m_ActiveDrawingMode;
    }

    public void ResetLineDrawing()
    {
        m_IsLineDrawingInProgress = false;
        m_StartingLineDrawingTilePosition = Vector2.zero;
        m_LineDrawingDirection = Vector2.zero;
    }

    public bool CanPaintTile(GridTile tile)
    {
        if (m_ActiveDrawingMode == EDrawingType.Line)
        {
            return TryLineDraw(tile);
        }

        //In free mode we can paint any tile
        return true;
    }

    private bool TryLineDraw(GridTile tile)
    {
        if (m_IsLineDrawingInProgress)
        {
            Vector2 currentTilePos = new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex());
            Vector2 normalizedDir = (currentTilePos - m_StartingLineDrawingTilePosition).normalized;
            if (new Vector2(Mathf.Abs(normalizedDir.x), Mathf.Abs(normalizedDir.y)) == new Vector2(Mathf.Abs(m_LineDrawingDirection.x), Mathf.Abs(m_LineDrawingDirection.y)))
            {
                return true;
            }
        }
        else
        {
            if (m_StartingLineDrawingTilePosition == Vector2.zero)
            {
                m_StartingLineDrawingTilePosition = new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex());
            }
            else
            {
                m_LineDrawingDirection = (new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex()) - m_StartingLineDrawingTilePosition).normalized;
                m_IsLineDrawingInProgress = true;
            }

            return true;
        }

        return false;
    }
}
