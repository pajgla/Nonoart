using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class CelebrationViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] Button m_ConfirmationButton;

        public Button GetConfirmationButton() { return m_ConfirmationButton; }
    }
}
