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
    DrawingController m_DrawingController = null;

    int m_LivesLeft = 3;
    bool m_CanDraw = false;
    int m_SolvedTiles = 0;
    bool m_IsPaused = false;

    //Time tracking
    float m_TimeTaken = 0.0f;

    [Header("Sprites")]
    [SerializeField] Sprite m_CrossSprite = null;
    [SerializeField] Sprite m_MarkSprite = null;
    [SerializeField] Sprite m_DefaultTileSprite = null;

    [Header("References")]
    [SerializeField] CelebrationPanelViewModel m_CelebrationPanelViewModelRef = null;
    [SerializeField] ControlBarViewModel m_ControlBarViewModelRef = null;
    [SerializeField] PauseViewModel m_PauseViewModelRef = null;

    //Viewmodels
    CelebrationPanelViewModel m_CelebrationPanelViewModel = null;
    ControlBarViewModel m_ControlBarViewModel = null;
    PauseViewModel m_PauseViewModel = null;

    public override void Init(GameModeData gameModeData)
    {
        base.Init(gameModeData);

        m_DrawingController = new DrawingController();

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

        m_ControlBarViewModel = ViewModelHelper.SpawnAndInitialize(m_ControlBarViewModelRef);
        m_ControlBarViewModel.SetDrawingModeImage(DrawingController.EDrawingType.Free);
        m_ControlBarViewModel.SetLives(m_LivesLeft);
        m_ControlBarViewModel.ChangeCreateButtonVisibility(false);
        m_ControlBarViewModel.GetPauseButton().onClick.AddListener(OnPauseButtonPressed);

        m_PauseViewModel = ViewModelHelper.SpawnAndInitialize(m_PauseViewModelRef);
        m_PauseViewModel.GetMainMenuButton().onClick.AddListener(OnGoToMainMenuButtonPressed);
        m_PauseViewModel.GetRestartButton().onClick.AddListener(OnRestartLevelButtonPressed);
        m_PauseViewModel.GetResumeButton().onClick.AddListener(OnResumeButtonPressed);
        m_PauseViewModel.ChangeViewModelVisibility(false);
    }

    private void Update()
    {
        //#TODO: This will only work on PC. Use new Input System for this
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLineDrawingMode();
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_DrawingController.ResetLineDrawing();
        }

        UpdateTimeTaken();
    }

    private void OnResumeButtonPressed()
    {
        m_PauseViewModel.ChangeViewModelVisibility(false);
        m_IsPaused = false;
        m_CanDraw = true;
    }

    private void OnRestartLevelButtonPressed()
    {
        GameManager.Get().ReloadCurrentLevel();
    }

    private void OnGoToMainMenuButtonPressed()
    {
        GameManager.Get().LoadMainMenu();
    }

    private void OnPauseButtonPressed()
    {
        m_PauseViewModel.ChangeViewModelVisibility(true);
        m_IsPaused = true;
        m_CanDraw = false;
    }

    private void ToggleLineDrawingMode()
    {
        if (m_IsPaused) 
            return;

        DrawingController.EDrawingType currentDrawingMode = m_DrawingController.ToggleDrawingMode();
        m_ControlBarViewModel.SetDrawingModeImage(currentDrawingMode);
    }

    private void UpdateTimeTaken()
    {
        if (m_IsPaused) 
            return;

        m_TimeTaken += Time.deltaTime;
        m_ControlBarViewModel.SetTime(m_TimeTaken);
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
        m_IsPaused = true;
        m_CanDraw = false;
        string nonogramID = m_NonogramToSolve.GetNonogramID();

        NonogramCompletionSaveData saveData = new NonogramCompletionSaveData();
        saveData.m_IsCompleted = true;
        //#TODO: Implement time tracking

        SavegameManager.Get().SaveNonogramData(nonogramID, saveData);

        m_CelebrationPanelViewModel.SetNonogramName(m_NonogramToSolve.GetNonogramName());
        m_CelebrationPanelViewModel.SetNonogramTexture(m_NonogramToSolve.GetAsTexture());
        m_CelebrationPanelViewModel.SetTotalTimeTaken(m_TimeTaken);
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
        if (m_DrawingController.CanPaintTile(tile))
        {
            PaintOrCrossTile(tile, isCrossed);
        }
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
        m_ControlBarViewModel.SetLives(m_LivesLeft);

        if (m_LivesLeft == 0)
        {
            m_CanDraw = false;
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        m_IsPaused = true;
        //#TODO: update UI
    }

    private void OnContinueButtonClicked()
    {
        //#TODO: Make custom scene loader
        GameManager.Get().LoadMainMenu();
    }

    private void OnDestroy()
    {
        GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
        m_CelebrationPanelViewModel.GetContinueButton().onClick.RemoveAllListeners();
        globalEvents.e_OnGridSpawned -= OnGridSpawnedCallback;
        globalEvents.e_OnTileClicked -= OnTileClicked;
        globalEvents.e_OnTileSolved -= OnTileSolved;
    }

#if UNITY_EDITOR
    public void Solve()
    {
        OnNonogramSolved();
    }

#endif
}
