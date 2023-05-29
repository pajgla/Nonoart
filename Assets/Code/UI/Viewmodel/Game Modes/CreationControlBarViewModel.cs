using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class CreationControlBarViewModel : ControlBarViewModel
    {
        [SerializeField] Button m_CreateButton = null;

        public Button GetCreateButton() { return m_CreateButton; }
    }
}
