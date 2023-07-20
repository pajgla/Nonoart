using System.Collections;
using System.Collections.Generic;
using UIViewModel;
using UnityEngine;

public abstract class DrawingGameMode : GameMode
{
    [Header("References")]
    [SerializeField] GridController m_GridControllerRef = null;
    [SerializeField] GridMovementController m_GridMovementControllerRef = null;
    [SerializeField] ControlBarViewModel m_ControlBarViewModelRef = null;
    [SerializeField] PauseViewModel m_PauseViewModelRef = null;

    //#TODO: Don't save sprites locally
    [Header("Sprites")]
    [SerializeField] protected Sprite m_CrossSprite = null;
    [SerializeField] protected Sprite m_MarkSprite = null;
    [SerializeField] protected Sprite m_DefaultTileSprite = null;

    //ViewModels
    protected ControlBarViewModel m_ControlBarViewModel = null;
    protected PauseViewModel m_PauseViewModel = null;

    protected GridController m_GridController = null;
    protected GridMovementController m_GridMovementController = null;

    protected DrawingController m_DrawingController = null;

    protected bool m_CanDraw = false;
    protected bool m_IsPaused = false;

    protected abstract void OnTileClicked(GridTile tile, KeyCode clickType);

    public override void Init(GameModeData gameModeData)
    {
        m_DrawingController = new DrawingController();

        m_GridController = Instantiate(m_GridControllerRef);
        m_GridController.Init();

        m_GridMovementController = Instantiate(m_GridMovementControllerRef);
        m_GridMovementController.Init(m_GridController.GetGridHolder());
        m_GridMovementController.SetCanDrag(true);

        GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
        globalEvents.e_OnGridSpawned += OnGridSpawnedCallback;
        globalEvents.e_OnTileClicked += OnTileClicked;

        InitializeViewModels();
    }

    protected virtual void InitializeViewModels()
    {
        m_ControlBarViewModel = ViewModelHelper.SpawnAndInitialize(m_ControlBarViewModelRef);
        m_ControlBarViewModel.SetDrawingModeImage(DrawingController.EDrawingType.Free); //default
        m_ControlBarViewModel.GetPauseButton().onClick.AddListener(OnPauseButtonPressed);

        m_PauseViewModel = ViewModelHelper.SpawnAndInitialize(m_PauseViewModelRef);
        m_PauseViewModel.GetMainMenuButton().onClick.AddListener(OnGoToMainMenuButtonPressed);
        m_PauseViewModel.GetRestartButton().onClick.AddListener(OnRestartLevelButtonPressed);
        m_PauseViewModel.GetResumeButton().onClick.AddListener(OnResumeButtonPressed);
        m_PauseViewModel.ChangeViewModelVisibility(false);
    }

    protected virtual void Update()
    {
        //#TODO: This will only work on PC. Use new Input System for this
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLineDrawingMode();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            m_DrawingController.ResetLineDrawing();
        }
    }


    private void ToggleLineDrawingMode()
    {
        if (m_IsPaused)
            return;

        DrawingController.EDrawingType currentDrawingMode = m_DrawingController.ToggleDrawingMode();
        m_ControlBarViewModel.SetDrawingModeImage(currentDrawingMode);
    }

    void OnGridSpawnedCallback()
    {
        m_CanDraw = true;
    }

    void OnPauseButtonPressed()
    {
        m_PauseViewModel.ChangeViewModelVisibility(true);
        m_IsPaused = true;
        m_CanDraw = false;
    }

    protected void OnRestartLevelButtonPressed()
    {
        GameManager.Get().ReloadCurrentLevel();
    }

    void OnResumeButtonPressed()
    {
        m_PauseViewModel.ChangeViewModelVisibility(false);
        m_IsPaused = false;
        m_CanDraw = true;
    }

    protected void OnGoToMainMenuButtonPressed()
    {
        GameManager.Get().LoadMainMenu();
    }

    protected void MarkTile(GridTile tile)
    {
        tile.SetForegroundImage(m_MarkSprite);
        tile.SetIsMarked(true);
    }

    protected virtual void OnDestroy()
    {
        GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
        globalEvents.e_OnGridSpawned -= OnGridSpawnedCallback;
        globalEvents.e_OnTileClicked -= OnTileClicked;
    }
}
