using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseSingleton<GameManager>
{
    [Header("References")]
    [SerializeField] GlobalEvents m_GlobalEventsRef = null;
    List<GameMode> m_GameModes = new List<GameMode>();

    [SerializeField] List<NonogramSet> m_NonogramSets = new List<NonogramSet>();

    GameMode m_CurrentGameMode = null;

    private void Start()
    {
        m_NonogramSets = NonogramHelpers.LoadAllNonograms();
    }

    public void StartGameMode(EGameModeType gameModeType)
    {
        StartCoroutine(LoadGameModeSceneAsync(gameModeType));
    }

    private IEnumerator LoadGameModeSceneAsync(EGameModeType gameModeType)
    {
        //#TODO:
        //-loading screen or fade in/fade out transition
        //-Use build index or something better instead of strings

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Nonogram Level");

        //Wait until the async scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //Scene loaded --------------

        //Find associated game mode object
        GameMode foundGameModeRef = null;
        foreach (GameMode gameMode in m_GameModes)
        {
            if (gameMode.GetGameModeType() == gameModeType)
            {
                foundGameModeRef = gameMode;
                break;
            }
        }

        if (foundGameModeRef == null)
        {
            Debug.LogError("Requested Game Mode type cannot be found in Game Modes list. Please add required game mode to the list or check for wrong values in existing game modes");
            yield return null;
        }

        GameMode newGameModeObj = Instantiate(foundGameModeRef);
        m_CurrentGameMode = newGameModeObj;
    }

    public bool IsInGameMode()
    {
        return m_CurrentGameMode != null;
    }

    //Getters
    public GlobalEvents GetGlobalEvents() { return m_GlobalEventsRef; }
}