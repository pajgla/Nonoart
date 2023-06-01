using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolvingGameMode : GameMode
{
    Nonogram m_NonogramToSolve = null;
    GridSpawner m_GridSpawner = null;
    GridMovementController m_GridMovementController = null;

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

    }
}
