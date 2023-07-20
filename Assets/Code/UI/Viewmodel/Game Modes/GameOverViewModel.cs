using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class GameOverViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] Button m_RetryButton = null;
        [SerializeField] Button m_GoToMainMenuButton = null;

        public Button GetRetryButton()
        {
            return m_RetryButton;
        }

        public Button GetGoToMainMenuButton()
        {
            return m_GoToMainMenuButton;
        }
    }
}
