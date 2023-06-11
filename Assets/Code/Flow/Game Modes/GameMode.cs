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

    [SerializeField] protected GridController m_GridController = null;
    [SerializeField] protected GridMovementController m_GridMovementControllerRef = null;

    public EGameModeType GetGameModeType() { return m_GameModeType; }

    public virtual void Init(GameModeData gameModeData)
    {

    }
}
