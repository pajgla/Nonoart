using System;
using UnityEngine;

public class SolvingGameMode : GameMode
{
    Nonogram m_NonogramToSolve = null;
    GridController m_GridSpawner = null;
    GridMovementController m_GridMovementController = null;

    //Line drawing
    bool m_LineDrawing = false;
    bool m_IsLineDrawingInProgress = false;
    Vector2 m_StartingLineDrawingTilePosition = Vector2.zero;
    Vector2 m_LineDrawingDirection = Vector2.zero;

    int m_LivesLeft = 3;
    bool m_CanDraw = false;

    [Header("Sprites")]
    [SerializeField] Sprite m_CrossSprite = null;
    [SerializeField] Sprite m_MarkSprite = null;
    [SerializeField] Sprite m_DefaultTileSprite = null;

    public override void Init(GameModeData gameModeData)
    {
        base.Init(gameModeData);

        m_GridSpawner = Instantiate(m_GridController);
        m_GridSpawner.Init();

        m_GridMovementController = Instantiate(m_GridMovementControllerRef);
        m_GridMovementController.Init(m_GridSpawner.GetGridHolder());

        m_NonogramToSolve = gameModeData.m_Nonogram;
        GameManager.Get().GetGlobalEvents().e_OnGridSpawned += OnGridSpawnedCallback;
        StartCoroutine(m_GridSpawner.SpawnGrid(m_NonogramToSolve));

        m_GridMovementController.SetCanDrag(true);
    }

    private void Update()
    {
        //#TODO: This will only work on PC. Use new Input System for this
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_LineDrawing = !m_LineDrawing;
            if (m_LineDrawing)
                ResetLineDrawing();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetLineDrawing();
        }
    }

    private void ResetLineDrawing()
    {
        m_IsLineDrawingInProgress = false;
        m_StartingLineDrawingTilePosition = Vector2.zero;
        m_LineDrawingDirection = Vector2.zero;
    }

    private void OnGridSpawnedCallback()
    {
        m_CanDraw = true;
    }

    private void Start()
    {
        GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
        globalEvents.e_OnTileClicked += OnTileClicked;
    }

    private void OnTileClicked(GridTile tile, KeyCode keyCode)
    {
        if (m_CanDraw == false || tile.GetIsSolved())
            return;

        if (keyCode == KeyCode.Mouse0)
        {
            OnTileClicked(tile, false);
        }
        else if (keyCode == KeyCode.Mouse1)
        {
            OnTileClicked(tile, true);
        }
        else if (keyCode == KeyCode.Mouse2)
        {
            if (!tile.GetIsMarked() || !tile.GetIsSolved())
                MarkTile(tile);
        }
    }

    private void MarkTile(GridTile tile)
    {
        //#TODO: Don't save sprites locally
        tile.SetForegroundImage(m_MarkSprite);
        tile.SetIsMarked(true);
    }

    private void OnTileClicked(GridTile tile, bool isCrossed)
    {
        if (m_LineDrawing && TryLaneDraw(tile))
        {
            PaintOrCrossTile(tile, isCrossed);
        }
        else
        {
            PaintOrCrossTile(tile, isCrossed);
        }
    }

    private bool TryLaneDraw(GridTile tile)
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

    private bool PaintOrCrossTile(GridTile tile, bool isTileCrossed)
    {
        if (tile.GetIsColored())
        {
            tile.Solve(tile.GetRequiredColor());
            tile.SetForegroundImage(m_DefaultTileSprite);
            GameManager.Get().GetGlobalEvents().Invoke_OnTilePainted(tile);
            if (isTileCrossed)
            {
                OnWrongGuess();
                return false;
            }
            else
                return true;
        }
        else
        {
            tile.SetForegroundImage(m_CrossSprite);
            tile.SetIsSolved(true);
            if (!isTileCrossed)
            {
                OnWrongGuess();
                return false;
            }
            else
                return true;
        }
    }

    private void OnWrongGuess()
    {
        m_LivesLeft--;

        //#TODO: Update UI

        if (m_LivesLeft == 0)
        {
            m_CanDraw = false;
        }
    }

    private void OnGameOver()
    {

    }
}
