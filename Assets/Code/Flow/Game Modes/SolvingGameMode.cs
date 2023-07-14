using Save;
using System;
using UIViewModel;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    int m_SolvedTiles = 0;

    [Header("Sprites")]
    [SerializeField] Sprite m_CrossSprite = null;
    [SerializeField] Sprite m_MarkSprite = null;
    [SerializeField] Sprite m_DefaultTileSprite = null;

    [Header("References")]
    [SerializeField] CelebrationPanelViewModel m_CelebrationPanelViewModelRef = null;

    CelebrationPanelViewModel m_CelebrationPanelViewModel = null;


    public override void Init(GameModeData gameModeData)
    {
        base.Init(gameModeData);

        //#TODO: Change names of GridSpawner and GridController
        m_GridSpawner = Instantiate(m_GridController);
        m_GridSpawner.Init();

        m_GridMovementController = Instantiate(m_GridMovementControllerRef);
        m_GridMovementController.Init(m_GridSpawner.GetGridHolder());

        m_NonogramToSolve = gameModeData.m_Nonogram;
        GameManager.Get().GetGlobalEvents().e_OnGridSpawned += OnGridSpawnedCallback;
        StartCoroutine(m_GridSpawner.SpawnGrid(m_NonogramToSolve));

        m_GridMovementController.SetCanDrag(true);

        m_CelebrationPanelViewModel = ViewModelHelper.SpawnAndInitialize(m_CelebrationPanelViewModelRef);
        m_CelebrationPanelViewModel.ChangeViewModelVisibility(false);
        m_CelebrationPanelViewModel.GetContinueButton().onClick.AddListener(OnContinueButtonClicked);
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
        globalEvents.e_OnTileSolved += OnTileSolved;
    }

    private void OnTileSolved(GridTile tile)
    {
        m_SolvedTiles++;
        if (m_SolvedTiles == m_GridSpawner.GetTotalTilesToSolve())
        {
            OnNonogramSolved();
        }
    }

    private void OnNonogramSolved()
    {
        m_CanDraw = false;
        string nonogramID = m_NonogramToSolve.GetNonogramID();

        NonogramCompletionSaveData saveData = new NonogramCompletionSaveData();
        saveData.m_IsCompleted = true;
        //#TODO: Implement time tracking

        SavegameManager.Get().SaveNonogramData(nonogramID, saveData);

        m_CelebrationPanelViewModel.SetNonogramName(m_NonogramToSolve.GetNonogramName());
        m_CelebrationPanelViewModel.SetNonogramTexture(m_NonogramToSolve.GetAsTexture());
        m_CelebrationPanelViewModel.SetTotalTimeTaken(70);
        m_CelebrationPanelViewModel.ChangeViewModelVisibility(true);
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

    private void OnContinueButtonClicked()
    {
        //#TODO: Make custom scene loader
        SceneManager.LoadScene(0);
    }
}
