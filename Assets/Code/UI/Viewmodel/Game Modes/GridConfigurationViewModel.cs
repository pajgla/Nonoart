using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class GridConfigurationViewModel : ViewModel
    {
        [SerializeField] UIValueSelector m_GridWidthValueSelector = null;
        [SerializeField] UIValueSelector m_GridHeightValueSelector = null;
        [SerializeField] Button m_StartButton = null;

        //Getters
        public UIValueSelector GetWidthValueSelector() { return m_GridWidthValueSelector;}
        public UIValueSelector GetHeightValueSelector() { return m_GridHeightValueSelector;}
        public Button GetStartButton() { return m_StartButton;}
    }
}
