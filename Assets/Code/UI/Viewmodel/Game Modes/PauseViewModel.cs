using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class PauseViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] Button m_MainMenuButton = null;
        [SerializeField] Button m_RestartButton = null;
        [SerializeField] Button m_ResumeButton = null;

        public Button GetMainMenuButton()
        {
            return m_MainMenuButton;
        }

        public Button GetRestartButton()
        {
            return m_RestartButton;
        }

        public Button GetResumeButton()
        {
            return m_ResumeButton;
        }
    }
}
