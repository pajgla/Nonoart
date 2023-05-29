using System.Collections;
using System.Collections.Generic;
using UIViewModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SC
{
    namespace MainMenuStates
    {
        [CreateAssetMenu(fileName = "Main Menu State", menuName = "Statechart/States/Main Menu/Main Menu State")]
        public sealed class MainMenuState : State
        {
            [Header("State References")]
            [SerializeField] MainMenuViewModel m_MainMenuVM = null;

            public override void OnEnter(Statechart controller)
            {
                base.OnEnter(controller);

                MainMenuViewModel newVM = ViewModelHelper.SpawnAndInitialize(m_MainMenuVM) as MainMenuViewModel;
                newVM.GetGoToLevelSelectionButton().onClick.AddListener(OnGoToLevelSelectionButtonCallback);
                newVM.GetGoToLevelCreationButton().onClick.AddListener(OnGoToLevelCreationButtonCallback);
            }

            private void OnGoToLevelSelectionButtonCallback()
            {

            }

            private void OnGoToLevelCreationButtonCallback()
            {
                SceneManager.LoadScene("Nonogram Creation Scene");
            }
        }

        public sealed class LevelSelectionState : State
        {
            //[Header("State References")]
            //[SerializeField] 
        }
    }
}
