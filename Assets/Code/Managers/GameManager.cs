using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseSingleton<GameManager>
{
    [Header("References")]
    [SerializeField] GlobalEvents m_GlobalEventsRef = null;
    [SerializeField] List<GameMode> m_GameModes = new List<GameMode>();

    [SerializeField] List<NonogramSet> m_NonogramSets = new List<NonogramSet>();

    GameMode m_CurrentGameMode = null;

    private void Start()
    {
        LoadNonogramSets();
    }

    public void LoadNonogramSets()
    {
        m_NonogramSets = NonogramHelpers.LoadAllNonograms();
    }

    public void StartGameMode(EGameModeType gameModeType, GameModeData gameModeData)
    {
        StartCoroutine(LoadGameModeSceneAsync(gameModeType, gameModeData));
    }

    private IEnumerator LoadGameModeSceneAsync(EGameModeType gameModeType, GameModeData gameModeData)
    {
        //#TODO:
        //-loading screen or fade in/fade out transition
        //-Use build index or something better instead of strings
        string sceneName = "Nonogram Level";
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

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
        m_CurrentGameMode.Init(gameModeData);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(m_CurrentGameMode);
        m_CurrentGameMode = null;
    }

    //Getters
    public GlobalEvents GetGlobalEvents() { return m_GlobalEventsRef; }
    public List<NonogramSet> GetNonogramSets() { return m_NonogramSets; }
    public bool IsInGameMode() { return m_CurrentGameMode != null; }
    public GameMode GetActiveGameMode() { return m_CurrentGameMode; }
}