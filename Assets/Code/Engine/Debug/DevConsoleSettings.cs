using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevConsole
{
    [CreateAssetMenu(fileName = "Dev Console Settings", menuName = "Dev Console/Settings")]
    public class DevConsoleSettings : ScriptableObject
    {
        [Header("Input")]
        public KeyCode m_ConsoleToggleInput = KeyCode.BackQuote;
        public KeyCode m_HistorySearchUpInput = KeyCode.UpArrow;
        public KeyCode m_HistorySearchDownInput = KeyCode.DownArrow;

        [Header("Settings")]
        public Color m_ErrorOutputColor = Color.red;
        public Color m_WarningOutputColor = Color.yellow;
        public Color m_DefaultOutputColor = Color.white;
    }
}
