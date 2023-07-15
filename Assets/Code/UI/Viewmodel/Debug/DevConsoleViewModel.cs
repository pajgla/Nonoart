using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UIViewModel
{
    public class DevConsoleViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI m_OutputText = null;
        [SerializeField] TMP_InputField m_InputField = null;

        public void AppendOutputString(string output, bool newLine = true)
        {
            string newOutput = string.Empty;

            newOutput += output;

            if (newLine)
                newOutput += "\n\n";

            m_OutputText.text += newOutput;
        }

        public TMP_InputField GetInputField() { return m_InputField; }
    }
}
