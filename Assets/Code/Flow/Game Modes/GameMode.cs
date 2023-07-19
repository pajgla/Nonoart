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

    public EGameModeType GetGameModeType() { return m_GameModeType; }

    public abstract void Init(GameModeData gameModeData);
}
