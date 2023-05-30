using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIViewModel;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MainMenuViewModel m_MainMenuViewModel = null;
    [SerializeField] LevelSelectionViewModel m_LevelSelectionViewModel = null;

    // Start is called before the first frame update
    void Start()
    {
        m_MainMenuViewModel = ViewModelHelper.SpawnAndInitialize(m_MainMenuViewModel);
        m_MainMenuViewModel.GetGoToLevelSelectionButton().onClick.AddListener(OnGoToLevelSelectionButtonCallback);
        m_MainMenuViewModel.GetGoToLevelCreationButton().onClick.AddListener(OnGoToLevelCreationButtonCallback);

        m_LevelSelectionViewModel = ViewModelHelper.SpawnAndInitialize(m_LevelSelectionViewModel);
        m_LevelSelectionViewModel.ChangeViewModelVisibility(false);
    }

    private void OnGoToLevelSelectionButtonCallback()
    {
        m_MainMenuViewModel.ChangeViewModelVisibility(false);
        m_LevelSelectionViewModel.ChangeViewModelVisibility(true);
    }

    private void OnGoToLevelCreationButtonCallback()
    {
        SceneManager.LoadScene("Nonogram Creation Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
