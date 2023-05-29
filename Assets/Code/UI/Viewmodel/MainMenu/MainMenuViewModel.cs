using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class MainMenuViewModel : ViewModel
    {
        [SerializeField] Button m_GoToLevelSelectionButton = null;
        [SerializeField] Button m_GoToLevelCreationButton = null;
        [SerializeField] Button m_OptionsButton = null;
        
        public Button GetGoToLevelSelectionButton() { return m_GoToLevelSelectionButton;}
        public Button GetGoToLevelCreationButton() {  return m_GoToLevelCreationButton;}
        public Button GetOptionsButton() { return m_OptionsButton;}
    }
}
