using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameModeType
{
    Creation,
    Solving,
}

public abstract class GameMode : MonoBehaviour
{
    [SerializeField] private EGameModeType m_GameModeType = EGameModeType.Creation;

    [SerializeField] protected GridSpawner m_GridSpawnerRef = null;
    [SerializeField] protected GridMovementController m_GridMovementControllerRef = null;

    public EGameModeType GetGameModeType() { return m_GameModeType; }
}
