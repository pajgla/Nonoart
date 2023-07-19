using Save;
using System;
using UIViewModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SolvingGameMode : DrawingGameMode
{
    Nonogram m_NonogramToSolve = null;

    int m_LivesLeft = 3;
    int m_SolvedTiles = 0;

    //Time tracking
    float m_TimeTaken = 0.0f;

    [Header("References")]
    [SerializeField] CelebrationPanelViewModel m_CelebrationPanelViewModelRef = null;

    //Viewmodels
    CelebrationPanelViewModel m_CelebrationPanelViewModel = null;

    public override void Init(GameModeData gameModeData)
    {
        base.Init(gameModeData);

        GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
        globalEvents.e_OnTileSolved += OnTileSolved;

        m_NonogramToSolve = gameModeData.m_Nonogram;
        StartCoroutine(m_GridController.SpawnGrid(m_NonogramToSolve));
    }

    protected override void InitializeViewModels()
    {
        base.InitializeViewModels();

        m_ControlBarViewModel.SetLives(m_LivesLeft);
        m_ControlBarViewModel.ChangeCreateButtonVisibility(false);

        m_CelebrationPanelViewModel = ViewModelHelper.SpawnAndInitialize(m_CelebrationPanelViewModelRef);
        m_CelebrationPanelViewModel.ChangeViewModelVisibility(false);
        m_CelebrationPanelViewModel.GetContinueButton().onClick.AddListener(OnContinueButtonClicked);
    }

    protected override void Update()
    {
        base.Update();

        UpdateTimeTaken();
    }

    private void UpdateTimeTaken()
    {
        if (m_IsPaused) 
            return;

        m_TimeTaken += Time.deltaTime;
        m_ControlBarViewModel.SetTime(m_TimeTaken);
    }

    private void OnTileSolved(GridTile tile)
    {
        m_SolvedTiles++;
        if (m_SolvedTiles == m_GridController.GetTotalTilesToSolve())
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

    protected override void OnTileClicked(GridTile tile, KeyCode keyCode)
    {
        if (m_CanDraw == false || tile.GetIsSolved() || !m_DrawingController.CanPaintTile(tile))
            return;

        if (keyCode == KeyCode.Mouse0)
        {
            PaintOrCrossTile(tile, false);
        }
        else if (keyCode == KeyCode.Mouse1)
        {
            PaintOrCrossTile(tile, true);
        }
        else if (keyCode == KeyCode.Mouse2)
        {
            if (!tile.GetIsMarked() && !tile.GetIsSolved())
                MarkTile(tile);
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

    protected override void OnDestroy()
    {
        GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
        globalEvents.e_OnTileSolved -= OnTileSolved;
    }

#if UNITY_EDITOR
    public void Solve()
    {
        OnNonogramSolved();
    }

#endif
}
