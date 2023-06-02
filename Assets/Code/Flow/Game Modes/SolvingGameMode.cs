using System.Collections;
using System.Collections.Generic;
using UIViewModel;
using UnityEngine;

public class SolvingGameMode : GameMode
{
    Nonogram m_NonogramToSolve = null;
    GridSpawner m_GridSpawner = null;
    GridMovementController m_GridMovementController = null;

    bool m_LineDrawing = false;
    bool m_IsLineDrawingInProgress = false;
    Vector2 m_StartingLineDrawingTilePosition = Vector2.zero;
    Vector2 m_LineDrawingDirection = Vector2.zero;

    public override void Init(GameModeData gameModeData)
    {
        base.Init(gameModeData);

        m_GridSpawner = Instantiate(m_GridSpawnerRef);
        m_GridSpawner.Init();

        m_GridMovementController = Instantiate(m_GridMovementControllerRef);
        m_GridMovementController.Init(m_GridSpawner.GetGridHolder());

        m_NonogramToSolve = gameModeData.m_Nonogram;
        StartCoroutine(m_GridSpawner.SpawnGrid(m_NonogramToSolve));

        m_GridMovementController.SetCanDrag(true);
    }

    private void Start()
    {
        GameManager.Get().GetGlobalEvents().e_OnTileClicked += OnTileClicked;
    }

    private void OnTileClicked(GridTile tile)
    {
        if (m_LineDrawing)
        {
            if (m_IsLineDrawingInProgress)
            {
                Vector2 currentTilePos = new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex());
                Vector2 normalizedDir = (currentTilePos - m_StartingLineDrawingTilePosition).normalized;
                print("Normalized Dir: " + normalizedDir);
                if (new Vector2(Mathf.Abs(normalizedDir.x), Mathf.Abs(normalizedDir.y)) == new Vector2(Mathf.Abs(m_LineDrawingDirection.x), Mathf.Abs(m_LineDrawingDirection.y)))
                {
                    TryPaintTile(tile);
                    return;
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
            }
        }

        TryPaintTile(tile);
    }

    private bool TryPaintTile(GridTile tile)
    {
        if (tile.GetIsColored())
        {
            tile.Paint(tile.GetRequiredColor());
            return true;
        }

        OnWrongGuess();
        return false;
    }

    public void OnWrongGuess()
    {

    }
}
